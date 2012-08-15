var express = require('express'),
  io = require('socket.io'),
  zmq = require('zmq'),
  routes = require('./routes')
  status = require('./routes/status')

var app = module.exports = express.createServer();

app.configure(function() {
  app.set('views', __dirname + '/views');
  app.set('view engine', 'jade');
  app.set('view options', {
    layout: false
  });
  app.use(express.static(__dirname + '/public/'));
});

app.configure('development', function() {
  app.use(express.errorHandler({
    dumpExceptions: true,
    showStack: true
  }));
});

app.configure('production', function() {
  app.use(express.errorHandler());
});

app.get('/:app/lib/*.js', function(req, res) {
  res.sendfile(__dirname + '/public/lib/js/' + req.params[0] + ".js");
});

app.get('/:app/lib/*.css', function(req, res) {
  res.sendfile(__dirname + '/public/lib/css/' + req.params[0] + ".css");
});
app.get('/status', routes.status.index);
app.get('/status/:name', routes.status.partials)

var server = app.listen(80, function() {
  console.log("Server is listening on port %d in %s mode", server.address().port, app.settings.env);
});

var ws = io.listen(server);
ws.on('connection', function(socket) {
  socket.on('connect:status', function(data) {
    routes.status.io(this, zmq);
  });
});
ws.sockets.on('status', routes.status.io);