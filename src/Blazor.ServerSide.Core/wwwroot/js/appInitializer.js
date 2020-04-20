window.appInitializer = {
    "callbacks": [],
    "subscribe": null,
    "onAppLoaded": null
}

window.appInitializer.callbacks = [];

window.appInitializer.subscribe = function(callback) {
    window.appInitializer.callbacks.push(callback);
}

window.appInitializer.onAppLoaded = function () {
    var callbacks = window.appInitializer.callbacks;
    for (var i = 0; i < callbacks.length; i++) {
        callbacks[i]();
    }
}