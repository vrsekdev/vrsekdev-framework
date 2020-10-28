window.vrsekdev = {};

window.vrsekdev.frest = {};

vrsekdev.frest.addLiviconEvo = (element, config) => {
    $element = $(element);

    $element.addLiviconEvo(config);
}

vrsekdev.frest.stopLiviconEvo = (element) => {
    $element = $(element);

    $element.stopLiviconEvo();
}

vrsekdev.frest.playLiviconEvo = (element) => {
    $element = $(element);

    $element.playLiviconEvo();
}