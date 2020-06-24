var token = $("[name='__RequestVerificationToken']").val();
var host = "octavian:21312";

$('#imgDots').click(function () {
    document.getElementById("optionsForm").style.display = "block";
});
function optionsDevicePopupClose() {
    document.getElementById("optionsForm").style.display = "none";
}
function removeDeviceClick() {
    if (confirm("Do you really want to remove this device?")) {
        var labelError = document.getElementById("odLabelError");
        var url = "/Devices/RemoveDevice";
        $.ajax({
            type: "POST",
            url: url,
            data: {
                __RequestVerificationToken: token,
                deviceID: document.getElementById("vID").value
            },
            success: function (data) {
                switch (data) {
                    case 0:
                        window.location.href = "https://" + window.location.hostname;
                        break;
                    case -1:
                        labelError.innerHTML = "Device ID is not valid. Please refresh the page and try again.";
                        break;
                    case -2:
                        labelError.innerHTML = "Something is wrong with your right for this device.";
                        break;
                    case -3:
                        labelError.innerHTML = "There was an internal database error. Please try again later.";
                        break;
                }
            },
            error: function () {
                alert("Error processing the request");
            }
        });

    }
}

function addDevicePopup() {
    document.getElementById("addForm").style.display = "block";
}
function addDevicePopupClose() {
    document.getElementById("addForm").style.display = "none";
    document.getElementById("adLabelErrorID").innerHTML = "";
    document.getElementById("adDeviceID").value = "";
    document.getElementById("adLabelErrorName").innerHTML = "";
    document.getElementById("adName").value = "";
    document.getElementById("adLabelErrorRoom").innerHTML = "";
    document.getElementById("adRoom").value = "";
}
function addDeviceClick() {
    var url = "/Devices/AddDevice";

    var labelErrorID = document.getElementById("adLabelErrorID");
    var deviceID = document.getElementById("adDeviceID");
    if (!deviceID.validity.valid) {
        labelErrorID.innerHTML = "Please fill out this filed.";
        return;
    }
    else {
        labelErrorID.innerHTML = "";
    }

    var labelErrorName = document.getElementById("adLabelErrorName");
    var name = document.getElementById("adName");
    if (!name.validity.valid) {
        labelErrorName.innerHTML = "Please fill out this filed.";
        return;
    }
    else {
        labelErrorName.innerHTML = "";
    }

    var labelErrorRoom = document.getElementById("adLabelErrorRoom");
    var room = document.getElementById("adRoom");
    if (!room.validity.valid) {
        labelErrorRoom.innerHTML = "Please fill out this filed.";
        return;
    }
    else {
        labelErrorRoom.innerHTML = "";
    }

    var deviceName = name.value;
    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            deviceID: deviceID.value,
            name: deviceName,
            room: room.value
        },
        success: function (data) {
            switch (data) {
                case 0:
                    addDevicePopupClose();
                    showDevice(deviceName, "True");
                    break;
                case -1:
                    labelErrorID.innerHTML = "Device ID is not valid. Please check it again.";
                    break;
                case -2:
                    labelErrorName.innerHTML = "You already have a device with this name. Please insert another unique name.";
                    break;
                case -3:
                    labelError.innerHTML = "There was an internal database error. Please try again later.";
                    break;
                case -4:
                    alert("Device alreay added by someone else");
                    break;
            }
        },
        error: function () {
            alert("Error processing the request");
        }
    });
}

window.onclick = function (event) {
    if (event.target == document.getElementById("addForm")) {
        this.addDevicePopupClose();
    }
    else if (event.target == document.getElementById("optionsForm")) {
        this.optionsDevicePopupClose();
    }
}

function showDevice(name, owned) {
    if (owned === "True") {
        window.location.href = "https://" + window.location.hostname + "/Devices/Show?name=" + name;
    }
}

function UpdateLight1(MAC, light1) {
    var url = "/Devices/UpdateLight1";
    var state1 = light1.checked;
    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            MAC: MAC,
            state1: state1
        },
        error: function () {
            alert("Error processing the request");
        }
    });
}

function UpdateLight2(MAC, light2) {
    var url = "/Devices/UpdateLight2";
    var state2 = light2.checked;
    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            MAC: MAC,
            state2: state2
        },
        error: function () {
            alert("Error processing the request");
        }
    });
}

$('#updateRoom').click(function () {
    var selectedRoom = document.getElementById("selectRoom").value;
    var room = "";
    if (selectedRoom != "ALL")
        room = "?room=" + selectedRoom;
    //window.location.href = "https://" + host + "/Devices" + room;
    window.location.href = "https://" + window.location.hostname + "/Devices" + room;
    //TODO addDevice redirect host knows.go.ro check
});

function roomClick(clickedRoom) {
    var room = "?room=" + clickedRoom;
    //window.location.href = "https://" + host + "/Devices" + room;
    window.location.href = "https://" + window.location.hostname + "/Devices" + room;
}

var vInputName = document.getElementById("vName");
var imgEdit = document.getElementById("imgEdit");
var imgOk = document.getElementById("imgOk");
var imgCancel = document.getElementById("imgCancel");
vInputName.addEventListener('keydown', keyDownInputName);
vInputName.addEventListener('mouseover', mousOverEditName);
vInputName.addEventListener('mouseleave', mousLeaveEditName);
vInputName.addEventListener('click', startEditName);
imgEdit.addEventListener('mouseover', mousOverEditName);
imgEdit.addEventListener('mouseleave', mousLeaveEditName);
imgEdit.addEventListener('click', startEditName);
imgOk.addEventListener('click', updateVName);
imgCancel.addEventListener('click', cancelEditName);
var nameValue = vInputName.innerHTML;
function keyDownInputName(e) {
    var keycode = event.which;
    if (keycode == '13') {
        updateVName();
        e.preventDefault();
    }
    else if (keycode == '27') {
        cancelEditName();
        e.preventDefault();
    }
}
function mousOverEditName() {
    if (imgOk.style.display !== "inline")
        imgEdit.style.display = "inline";
}
function mousLeaveEditName() {
    imgEdit.style.display = "none";
}
function startEditName() {
    vInputName.focus();
    imgEdit.style.display = "none";
    imgOk.style.display = "inline";
    imgCancel.style.display = "inline";
}
function cancelEditName() {
    imgOk.style.display = "none";
    imgCancel.style.display = "none";
    vInputName.innerHTML = nameValue;
    vInputName.blur();
    document.getElementById("vErrorName").innerHTML = "";
}
function updateVName() {
    var url = "/Devices/UpdateName";
    var labelError = document.getElementById("vErrorName");
    if (vInputName.innerHTML === "") {
        labelError.innerHTML = "Please insert an unique name.";
        vInputName.focus();
        return;
    }
    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            deviceID: document.getElementById("vID").value,
            name: vInputName.innerHTML
        },
        success: function (data) {
            //alert(data);
            switch (data) {
                case 0:
                    imgOk.style.display = "none";
                    imgCancel.style.display = "none";
                    nameValue = vInputName.innerHTML;
                    //window.history.replaceState("string", "Show", “/another-new-url”);
                    document.location.href = "Show?name=" + nameValue;
                    labelError.innerHTML = "";
                    break;
                case -2:
                    labelError.innerHTML = "You already have a device with this name. Please insert another unique name.";
                    vInputName.focus();
                    vInputName.select();
                    break;
                case -3:
                    labelError.innerHTML = "There was an internal database error. Please try again later.";
                    break;
            }
        },
        error: function () {
            alert("Error processing the request");
        }
    });
}

var vShareEmail = document.getElementById("vShareEmail");
vShareEmail.addEventListener('keydown', keyDownInputShareEmail);
vShareEmail.addEventListener('keyup', keyUpInputShareEmail);
function keyDownInputShareEmail(e) {
    var keycode = event.which;
    if (keycode == '13') {
        shareEmail();
        e.preventDefault();
    }
    if (!vShareEmail.value)
        document.getElementById("vErrorShareEmail").innerHTML = "";
}
function keyUpInputShareEmail() {
    if (!vShareEmail.value)
        document.getElementById("vErrorShareEmail").innerHTML = "";
}
function shareEmail() {
    var email = vShareEmail.value;
    var url = "/Devices/ShareDevice";
    var labelError = document.getElementById("vErrorShareEmail");
    if (email === "") {
        labelError.innerHTML = "Please insert an email.";
        vShareEmail.focus();
        return;
    }
    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            deviceID: document.getElementById("vID").value,
            email: email
        },
        success: function (data) {
            //alert(data);
            switch (data) {
                case 0:
                    vShareEmail.value = "";
                    vShareEmail.blur();
                    var listEmails = document.getElementById("listEmails");
                    var div = document.createElement("div");
                    div.className = "share-item";
                    div.innerHTML = "&bull;&nbsp;<button class=\"nav-link text dark share-email\" onclick=\"removeShare('"+email+"', this)\">"+email+"</button><img id=\"imgDelete\" class=\"img-delete\" src=\"/images/cancel.png\" alt=\"cancel\" onclick=\"removeShare('"+email+"', this)\" />";
                    listEmails.appendChild(div);
                    labelError.innerHTML = "";
                    break;
                case -1:
                    labelError.innerHTML = "Email is invalid.";
                    vShareEmail.focus();
                    vShareEmail.select();
                    break;
                case -2:
                    labelError.innerHTML = "Already sharing with this email.";
                    vShareEmail.focus();
                    vShareEmail.select();
                    break;
                case -3:
                    labelError.innerHTML = "There was an internal database error. Please try again later.";
                    break;
            }
        },
        error: function () {
            alert("Error processing the request");
        }
    });
}
function removeShare(email, elem) {
    if (confirm("Do you really want to unshare this device?")) {
        var url = "/Devices/RemoveShare";
        $.ajax({
            type: "POST",
            url: url,
            data: {
                __RequestVerificationToken: token,
                deviceID: document.getElementById("vID").value,
                email: email
            },
            success: function (data) {
                //alert(data);
                switch (data) {
                    case 0:
                        elem.parentElement.remove();
                        break;
                    case -3:
                        labelError.innerHTML = "There was an internal database error. Please try again later.";
                        break;
                }
            },
            error: function () {
                alert("Error processing the request");
            }
        });
    }
}




//$("button").click(function () {
//    alert("CLICKIED");
//});

//document.getElementById("light2").onclick = function (event) {
//}

//document.getElementById("clc").addEventListener("click", clck);
//function clck(event) {
//}