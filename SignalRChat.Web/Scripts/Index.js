var chat = {};

$(function () {
    // Reference the auto-generated proxy for the hub.
    chat = $.connection.chatHub;
    $.connection.hub.logging = true;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {
        // Add the message to the page.
        var msj = "<li class='media'><div class='media-body'><div class='media'> <div class='circle-small' style='float:left;background:" + colorUserChat + "'> " + name.substring(0, 1) + " </div> <div class='media-body'> " + htmlEncode(message) + " <br /><small class='text-muted'>" + htmlEncode(name) + " | " + new Date().toLocaleString("es-AR") + "</small><hr /></div></div></div></li>"

        $("#chatRoom").append(msj);
    };

    // Create a function that the hub can call back to display messages.
    chat.client.audioRecognized = function (message) {
        chat.server.send($("#chatUser").val(), message);
    };

    // Get the user name and store it to prepend to messages.
    bootbox.prompt("Ingrese su nombre:", function(result) {
        $("#chatUser").val(result);
        chat.server.login($("#chatUser").val());
    });

    chat.client.newUser = function (userName) {
        notificarIngreso(userName);
    };

    var colorUserChat = getRandomColor();
    $("#colorUserChat").val(colorUserChat);
    // Set initial focus to message input box.
    $("#message").focus();

    // Start the connection.
    $.connection.hub.start().done(function () {

        $("#message").on("keyup", function (e) {
            if (e.keyCode === 13) {
                if ($("#message").val() != "") {
                    // Call the Send method on the hub.
                    chat.server.send($("#chatUser").val(), $("#message").val());
                    // Clear text box and reset focus for next comment.
                    $("#message").val("").focus();
                } else {
                    bootbox.alert({
                        title: "Mensaje",
                        message: "Ingrese su mensaje"
                    });
                }
            }
        });
    });
});

// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $("<div />").text(value).html();
    return encodedValue;
};

function getRandomColor() {
    var letters = "0123456789ABCDEF";
    var color = "#";
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
};

function notificarIngreso(userName) {
    $.notify({
        // options
        icon: 'fa fa-user',
        title: userName,
        message: " está conectado al chat.",
    }, {
            // settings
            element: 'body',
            position: null,
            type: "info",
            allow_dismiss: true,
            newest_on_top: false,
            showProgressbar: false,
            placement: {
                from: "top",
                align: "right"
            },
            offset: 20,
            spacing: 10,
            z_index: 1031,
            delay: 5000,
            timer: 1000,
            mouse_over: null,
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            onShow: null,
            onShown: null,
            onClose: null,
            onClosed: null,
            icon_type: 'class',
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1}</span> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress" data-notify="progressbar">' +
            '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
            '</div>'
        });
}

$("#micro").mouseup(function () {
    stopRecording(this);
}).mousedown(function () {
    startRecording(this);
});

function __log(e, data) {
    //log.innerHTML += "\n" + e + " " + (data || '');

    console.log(e + " - " + (data || ""));
};

var audio_context;
var recorder;

function startUserMedia(stream) {
    var input = audio_context.createMediaStreamSource(stream);
    __log("Media stream created.");

    // Uncomment if you want the audio to feedback directly
    //input.connect(audio_context.destination);
    //__log('Input connected to audio context destination.');

    recorder = new Recorder(input);
    __log("Recorder initialised.");
}

function startRecording(button) {
    recorder && recorder.record();
    //button.disabled = true;
    //button.nextElementSibling.disabled = false;
    __log("Recording...");
}

function stopRecording(button) {
    recorder && recorder.stop();
    //button.disabled = true;
    //button.previousElementSibling.disabled = false;
    __log("Stopped recording.");

    // create WAV download link using audio data blob
    createDownloadLink();

    recorder.clear();
}

function createDownloadLink() {
    recorder && recorder.exportWAV(function (blob) {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", urls.speech + "&connectionId=" + chat.connection.id, true);
        xhr.setRequestHeader("content-type", "audio/wav");
        xhr.onload = function (e) {
            console.log(e);
        }
        xhr.send(blob);
    });
}

window.onload = function init() {
    try {
        // webkit shim
        window.AudioContext = window.AudioContext || window.webkitAudioContext;
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia;
        window.URL = window.URL || window.webkitURL;

        audio_context = new AudioContext;
        __log("Audio context set up.");
        __log("navigator.getUserMedia " + (navigator.getUserMedia ? "available." : "not present!"));
    } catch (e) {
        alert("No web audio support in this browser!");
    }

    navigator.getUserMedia({ audio: true }, startUserMedia, function (e) {
        __log("No live audio input: " + e);
    });
};