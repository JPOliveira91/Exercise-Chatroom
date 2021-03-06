"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

document.addEventListener('DOMContentLoaded', function () {
    var li = document.createElement("li");
    var room = document.getElementById("hdnCurrentRoom").value;
    var user = document.getElementById("hdnUserName").value;
    var messagesList = document.getElementById("messagesList" + room);

    messagesList.appendChild(li);
    li.innerHTML = `<li><label style="font-style: italic;">Welcome to ` + room + ` chatroom, ` + user + `</label></li>`;
}, false);

connection.on("ReceiveMessage", function (room, user, date, message) {
    var li = document.createElement("li");
    var messagesList = document.getElementById("messagesList" + room);

    if (messagesList.children.length >= 50)
    {
        messagesList.removeChild(messagesList.lastChild);
    }

    messagesList.insertBefore(li, messagesList.firstChild);

    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    //li.textContent = `(${date}) ${user}: ${message}`;
    li.innerHTML = `(<label style="font-style: italic;">${date}</label>) <label style="font-weight: bold;">${user}</label>: ${message}`;
    document.getElementById("messageInput").value = "";
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {    
    var room = document.getElementById("hdnCurrentRoom").value;
    var user = document.getElementById("hdnUserName").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", room, user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});