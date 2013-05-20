$(function(){
  var connection = $.connection('/logs');

  connection.received(function(data) {
    console.log(data);
  });

  connection.start().done(function() {
  });
});
