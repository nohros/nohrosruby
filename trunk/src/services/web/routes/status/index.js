exports.index = function(req, res) {
  res.render('status/index');
};

exports.partials = function(req, res) {
  res.render('status/' + req.params.name);
};

exports.io = function(socket, zmq) {
  // Each socket should be associated with a zmq subscriber socket.
  var subscriber = zmq.socket('sub');
  subscriber.on('message', function(feed, msg) {
    socket.emit('message', msg.toString());
  });

  // connects the subscriber to the publisher
  subscriber.connect('tcp://192.168.203.252:8523');

  // allow clients to subscribe to the topics of your own interest.
  socket.on('subscribe', function(topic) {
    subscriber.subscribe(topic);
  });

  socket.on('unsubscribe', function() {
    subscriber.unsubscribe(topic);
  });

  socket.on('disconnect', function() {
    subscriber.close();
  });
};
