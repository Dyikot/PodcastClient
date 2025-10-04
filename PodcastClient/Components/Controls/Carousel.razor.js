function ScrollCarouselLeft(container) {
    container.scrollBy(-container.clientWidth, 0);
}

function ScrollCarouselRight(container) {
    container.scrollBy(container.clientWidth, 0);
}