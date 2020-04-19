window.jsInjector = {
    "injectJsFile": null,
    "injectJsFiles": null
}

window.jsInjector.injectJsFile = function (filename) {
    // Get the first script element on the page
    var ref = window.document.getElementsByTagName('script')[0];
    // Create a new script element
    var script = window.document.createElement('script');
    // Set the script element `src`
    script.src = filename;
    // Inject the script into the DOM
    ref.parentNode.insertBefore(script, ref);
}

window.jsInjector.injectJsFiles = function (filenames) {
    if (filenames.length > 0) {
        loadNext(filenames, 0);
    }

    function loadNext(filenames, index) {
        var callback = function () { };
        if (filenames.length > index + 1) {
            callback = function () { loadNext(filenames, index += 1); }
        }
        loadScript(filenames[index], callback)
    }
}

function loadScript(url, callback) {
    // Get the first script element on the page
    var ref = window.document.getElementsByTagName('script')[0];

    var script = document.createElement("script")
    script.type = "text/javascript";

    if (script.readyState) {  //IE
        script.onreadystatechange = function () {
            if (script.readyState == "loaded" ||
                script.readyState == "complete") {
                script.onreadystatechange = null;
                callback();
            }
        };
    } else {  //Others
        script.onload = function () {
            callback();
        };
    }

    script.src = url;
    ref.parentNode.insertBefore(script, ref);
}