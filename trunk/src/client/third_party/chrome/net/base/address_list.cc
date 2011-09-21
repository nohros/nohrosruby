// Copyright (c) 2011 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#include "net/base/address_list.h"

#include <stdlib.h>

#include "common/logging.h" // replaced by the google-glog implementation.

#include "net/base/net_util.h"
#include "net/base/sys_addrinfo.h"

namespace net {

namespace {

char* do_strdup(const char* src) {
#if defined(OS_WIN)
  return _strdup(src);
#else
  return strdup(src);
#endif
}

struct addrinfo* CreateAddrInfo(const IPAddressNumber& address,
                                bool canonicalize_name) {
  struct addrinfo* ai = new addrinfo;
  memset(ai, 0, sizeof(addrinfo));
  ai->ai_socktype = SOCK_STREAM;

  switch (address.size()) {
    case kIPv4AddressSize: {
      ai->ai_family = AF_INET;
      const size_t sockaddr_in_size = sizeof(struct sockaddr_in);
      ai->ai_addrlen = sockaddr_in_size;

      struct sockaddr_in* addr = reinterpret_cast<struct sockaddr_in*>(
          new char[sockaddr_in_size]);
      memset(addr, 0, sockaddr_in_size);
      addr->sin_family = AF_INET;
#if defined(OS_MACOSX)
      addr->sin_len = sockaddr_in_size;
#endif
      memcpy(&addr->sin_addr, &address[0], kIPv4AddressSize);
      ai->ai_addr = reinterpret_cast<struct sockaddr*>(addr);
      break;
    }
    case kIPv6AddressSize: {
      ai->ai_family = AF_INET6;
      const size_t sockaddr_in6_size = sizeof(struct sockaddr_in6);
      ai->ai_addrlen = sockaddr_in6_size;

      struct sockaddr_in6* addr6 = reinterpret_cast<struct sockaddr_in6*>(
          new char[sockaddr_in6_size]);
      memset(addr6, 0, sockaddr_in6_size);
      addr6->sin6_family = AF_INET6;
#if defined(OS_MACOSX)
      addr6->sin6_len = sockaddr_in6_size;
#endif
      memcpy(&addr6->sin6_addr, &address[0], kIPv6AddressSize);
      ai->ai_addr = reinterpret_cast<struct sockaddr*>(addr6);
      break;
    }
    default: {
      NOTREACHED() << "Bad IP address";
      break;
    }
  }

  if (canonicalize_name) {
    std::string name = NetAddressToString(ai);
    ai->ai_canonname = do_strdup(name.c_str());
  }
  return ai;
}

}  // namespace

AddressList::AddressList() {
}

AddressList::AddressList(const AddressList& addresslist)
    : head_(Copy addresslist.head_) {
}

AddressList::~AddressList() {
}

AddressList& AddressList::operator=(const AddressList& addresslist) {
  head_ = addresslist.head_;
  return *this;
}

// static
AddressList AddressList::CreateFromIPAddressList(
    const std::vector<IPAddressNumber>& addresses,
    uint16 port) {
  DCHECK(!addresses.empty());
  struct addrinfo* head = NULL;
  struct addrinfo* next = NULL;

  for (std::vector<IPAddressNumber>::const_iterator it = addresses.begin();
       it != addresses.end(); ++it) {
    if (head == NULL) {
      head = next = CreateAddrInfo(*it, false);
    } else {
      next->ai_next = CreateAddrInfo(*it, false);
      next = next->ai_next;
    }
  }

  SetPortForAllAddrinfos(head, port);
  return AddressList(head, false);
}

// static
AddressList AddressList::CreateFromIPAddress(
      const IPAddressNumber& address,
      uint16 port) {
  return CreateFromIPAddressWithCname(address, port, false);
}

// static
AddressList AddressList::CreateFromIPAddressWithCname(
    const IPAddressNumber& address,
    uint16 port,
    bool canonicalize_name) {
  struct addrinfo* ai = CreateAddrInfo(address, canonicalize_name);

  SetPortForAllAddrinfos(ai, port);
  return AddressList(ai, false /*is_system_created*/);
}


// static
AddressList AddressList::CreateByAdoptingFromSystem(struct addrinfo* head) {
  return AddressList(head, true /* is system created */);
}

// static
AddressList AddressList::CreateByCopying(const struct addrinfo* head) {
  return AddressList(CreateCopyOfAddrinfo(head, true /*recursive*/), false /*is_system_created*/);
}

// static
AddressList AddressList::CreateByCopyingFirstAddress(
    const struct addrinfo* head) {
  return AddressList(CreateCopyOfAddrinfo(head, false /*recursive*/),
                              false /*is_system_created*/);
}

// static
AddressList AddressList::CreateFromSockaddr(
    const struct sockaddr* address,
    socklen_t address_length,
    int socket_type,
    int protocol) {
  // Do sanity checking on socket_type and protocol.
  DCHECK(socket_type == SOCK_DGRAM || socket_type == SOCK_STREAM);
  DCHECK(protocol == IPPROTO_TCP || protocol == IPPROTO_UDP);

  struct addrinfo* ai = new addrinfo;
  memset(ai, 0, sizeof(addrinfo));
  switch (address_length) {
    case sizeof(struct sockaddr_in):
      {
        const struct sockaddr_in* sin =
            reinterpret_cast<const struct sockaddr_in*>(address);
        ai->ai_family = sin->sin_family;
        DCHECK_EQ(AF_INET, ai->ai_family);
      }
      break;
    case sizeof(struct sockaddr_in6):
      {
        const struct sockaddr_in6* sin6 =
            reinterpret_cast<const struct sockaddr_in6*>(address);
        ai->ai_family = sin6->sin6_family;
        DCHECK_EQ(AF_INET6, ai->ai_family);
      }
      break;
    default:
      NOTREACHED() << "Bad IP address";
      break;
  }
  ai->ai_socktype = socket_type;
  ai->ai_protocol = protocol;
  ai->ai_addrlen = address_length;
  ai->ai_addr = reinterpret_cast<struct sockaddr*>(new char[address_length]);
  memcpy(ai->ai_addr, address, address_length);
  return AddressList(ai, false /*is_system_created*/);
}

void AddressList::SetPort(uint16 port) {
  // NOTE: we need to be careful not to mutate the reference-counted data,
  // since it might be shared by other AddressLists.
  SetPortForAllAddrinfos(head_, port);
}

uint16 AddressList::GetPort() const {
  return GetPortFromAddrinfo(head_);
}

bool AddressList::GetCanonicalName(std::string* canonical_name) const {
  DCHECK(canonical_name);
  if (!data_ || !data_->head->ai_canonname)
    return false;
  canonical_name->assign(data_->head->ai_canonname);
  return true;
}

const struct addrinfo* AddressList::head() const {
  if (!data_)
    return NULL;
  return data_->head;
}

AddressList::AddressList(Data* data) : data_(data) {}

AddressList::Data::Data(struct addrinfo* ai, bool is_system_created)
    : head(ai), is_system_created(is_system_created) {
  DCHECK(head);
}

AddressList::Data::~Data() {
  // Casting away the const is safe, since upon destruction we know that
  // no one holds a reference to the data any more.
  struct addrinfo* mutable_head = const_cast<struct addrinfo*>(head);

  // Call either freeaddrinfo(head), or FreeMyAddrinfo(head), depending who
  // created the data.
  if (is_system_created)
    freeaddrinfo(mutable_head);
  else
    FreeCopyOfAddrinfo(mutable_head);
}

}  // namespace net
