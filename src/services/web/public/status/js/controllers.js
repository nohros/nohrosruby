var MAX_NUMBER_OF_SLOTS = 10;
var LABEL_CSS_TEXT_MAP = {
  debug:  {
    css: "inverse",
    text: "Debug"
  },
  fatal: {
    css: "inverse",
    text: "Fatal"
  },
  warning: {
    css: "warning",
    text: "Warning"
  },
  warn: this.warning,
  info: {
    css: "info",
    text: "Info"
  },
  error: {
    css: "important",
    text: "Error"
  },

  label: function(name) {
    return this[name];
  }
};

function StatusCtrl($scope, socket, $log, $location) {
  var services_ = {};

  $scope.services = [];
  $scope.detail = function(index) {
    $location.path('/detail/' + index);
  }

  // Connects the socket to the 'STATUS' app.
  socket.emit('connect:status');

  // subscribe to the JSON feed at start to receive messages from
  // all services. Users can add/remove subscription latter
  socket.emit('subscribe', 'json');

  // Wait for messages from web server.
  socket.on('message', function(data) {
    try {
      var message = JSON.parse(data);
    } catch(e) {
      // TODO(neylor.silva) Display the error to the user.
      $log.error("received invalid data from server. data: "
        + data);
      return;
    }

    // service objects are services when the first message arrives.
    var hash = ServiceHash(message.pid, message.host, message.application);
    var service = services_[hash];
    if (service == null) {
      service = services_[hash] = new Service(message.pid, message.host, message.application);
      $scope.services.push(service);
      console.log(service);
    }

    service.states().add(message);
  });
}

function StatusDetailCtrl($scope, $routeParams) {
  $scope.name = "detail";
}

function ServiceHash(pid, host, name) {
  return "host-" + host + "-name-" + name;
}

function Service(pid, host, name) {
  this.name_ = name;
  this.pid_ = pid;
  this.host_ = host;
  this.states_ = new States(MAX_NUMBER_OF_SLOTS);
  this.hash_ = ServiceHash(pid, host, name);
}

Service.prototype = {
  states: function() {
    return this.states_;
  },

  state: function(index) {
    return this.states_.get(index);
  },

  host : function() {
    return this.host_;
  },

  pid: function() {
    return this.pid_;
  },

  name: function() {
    return this.name_;
  },

  hash: function() {
    return this.hash_;
  }
}

function States(length) {
  this.states_ = new Array();
  this.currentSlot_ = -1;
}

States.prototype = {
  add: function (message) {
    if (++this.currentSlot_ > 10) {
      this.currentSlot_ = 0;
    }
    this.states_[this.currentSlot_] = new State(message);
  },

  get: function(index) {
    if(index == undefined) {
      index = this.currentSlot_;
    }
    return this.states_[index];
  },

  all: function() {
    return this.states_;
  }
};

/**
 * Creates an new instance of the State using the specified
 * log state message.
 *
 * @param message A log message that contains the state data.
 * @constructor
 */
function State(message) {
  this.message_ = message;
}

State.prototype = {
  /**
   * Gets the state name.
   * \p This value represents the level of the log message.
   * @return {*}
   */
  name: function() {
    return this.message_.level;
  },

  /**
   * Gets the state description.
   * @param length
   * @return {String}
   */
  description: function(length) {
    if (angular.isNumber(length) && length > 0 && length < this.message_.reason.length) {
      return this.message_.reason.substring(0, length-3) + "...";
    }
    return this.message_.reason;
  },

  /**
   * Gets a collection of key value pairs containing state
   * specific information.
   * @return {Function}
   */
  categorization: function() {
    return this.message_.categorization;
  },

  /**
   * Gets the state's timestamp formatted accordingly to the
   * specified format. Defaults to ISO-8601.
   * @param format
   */
  timestamp: function(format) {
    return moment(this.message_.timestamp).utc().format(format);
  },

  label: function(type) {
    return LABEL_CSS_TEXT_MAP.label(this.message_.level)[type];
  }
}