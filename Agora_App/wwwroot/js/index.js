// Create Agora client
var client = AgoraRTC.createClient({
    mode: "rtc",
    codec: "vp8"
});

// RTM Global Vars
var isLoggedIn = false;

var localTracks = {
    videoTrack: null,
    audioTrack: null
};
var remoteUsers = {};
// Agora client options
var options = {
    appid: null,
    channel: null,
    uid: null,
    token: null,
    accountName: null
};

$("#join-form").submit(async function (e) {
    e.preventDefault();
    $("#join").attr("disabled", true);
    try {
        options.appid = $("#appid").val();
        options.token = $("#token").val();
        options.channel = $("#channel").val();
        options.accountName = $('#accountName').val();
        await join();
    } catch (error) {
        console.error(error);
    } finally {
        $("#leave").attr("disabled", false);
    }
})

$("#leave").click(function (e) {
    leave();
})

async function join() {
    debugger
    $("#mic-btn").prop("disabled", false);
    $("#video-btn").prop("disabled", false);
    document.getElementById('mic-icon').setAttribute("src", "/images/mute.svg");
    document.getElementById('video-icon').setAttribute('src', '/images/video on.svg')
    // add event listener to play remote tracks when remote user publishs.
    client.on("user-published", handleUserPublished);
    client.on("user-left", handleUserLeft);
    // join a channel and create local tracks, we can use Promise.all to run them concurrently
    [options.uid, localTracks.audioTrack, localTracks.videoTrack] = await Promise.all([
        // join the channel
        client.join(options.appid, options.channel, options.token || null),
        // create local tracks, using microphone and camera
        AgoraRTC.createMicrophoneAudioTrack(),
        AgoraRTC.createCameraVideoTrack()
    ]);
    // play local video track
    localTracks.videoTrack.play("local-player");
    $("#local-player-name").text(`localVideo(${options.uid})`);
    // publish local tracks to channel
    await client.publish(Object.values(localTracks));
    console.log("publish success");
}
async function leave() {
    for (trackName in localTracks) {
        var track = localTracks[trackName];
        if (track) {
            track.stop();
            track.close();
            $('#mic-btn').prop('disabled', true);
            $('#video-btn').prop('disabled', true);
            localTracks[trackName] = undefined;
        }
    }
    // remove remote users and player views
    remoteUsers = {};
    $("#remote-playerlist").html("");
    // leave the channel
    await client.leave();
    $("#local-player-name").text("");
    $("#join").attr("disabled", false);
    $("#leave").attr("disabled", true);
    console.log("client leaves channel success");
}

async function subscribe(user, mediaType) {
    debugger
    const uid = user.uid;
    $("#remote-playerlist").html('');
    // subscribe to a remote user
    await client.subscribe(user, mediaType);
    console.log("subscribe success");
    if (mediaType === 'video') {
        const player = $(`
      <div id="player-wrapper-${uid}">
        <p class="player-name">remoteUser(${uid})</p>
        <div id="player-${uid}" class="player"></div>
      </div>
    `);
        $("#remote-playerlist").append(player);
        user.videoTrack.play(`player-${uid}`);
    }
    if (mediaType === 'audio') {
        user.audioTrack.play();
    }
}

// Handle user publish
function handleUserPublished(user, mediaType) {
    const id = user.uid;
    remoteUsers[id] = user;
    subscribe(user, mediaType);
}

// Handle user left
function handleUserLeft(user) {
    const id = user.uid;
    delete remoteUsers[id];
    $(`#player-wrapper-${id}`).remove();
}

// Initialise UI controls
enableUiControls();

// Action buttons
function enableUiControls() {
    $("#mic-btn").click(function () {
       var img = document.getElementById('mic-btn').getElementsByTagName("img")[0].getAttribute("src");
       var imgName= img.split('/')[2];
        debugger
        toggleMic(imgName);
    });
    $("#video-btn").click(function () {
        var img = document.getElementById('video-btn').getElementsByTagName("img")[0].getAttribute("src");
        var imgName = img.split('/')[2];
        toggleVideo(imgName);
    });
}

// Toggle Mic
function toggleMic(imgName) {
   // if ($("#mic-icon").hasClass('fa-microphone')) {
    if (imgName =="mute.svg") {
        localTracks.audioTrack.setEnabled(false);
        document.getElementById('mic-icon').setAttribute('src', '/images/Unmute.svg')
        console.log("Audio Muted.");
    } else {
        localTracks.audioTrack.setEnabled(true);
        document.getElementById('mic-icon').setAttribute("src", "/images/mute.svg");
        console.log("Audio Unmuted.");
    }
    //$("#mic-icon").toggleClass('fa-microphone').toggleClass('fa-microphone-slash');
}

// Toggle Video
function toggleVideo(imgName) {
    debugger
    //if ($("#video-icon").hasClass('fa-video-camera')) {
    if (imgName == "video on.svg") {
        localTracks.videoTrack.setEnabled(false);
        document.getElementById('video-icon').setAttribute('src', '/images/video off.svg')
        console.log("Video Muted.");
    } else {
        localTracks.videoTrack.setEnabled(true);
        document.getElementById('video-icon').setAttribute('src', '/images/video on.svg')
        console.log("Video Unmuted.");
    }
   // $("#video-icon").toggleClass('fa fa-video-camera').toggleClass('fas fa-video-slash');
}