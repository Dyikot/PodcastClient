function InitializeCarousel(carousel, leftButtonId, rightButtonId) {
    const leftButton = document.getElementById(leftButtonId);
    const rightButton = document.getElementById(rightButtonId);

    leftButton.addEventListener("click", () => {
        carousel.scrollBy(-carousel.clientWidth, 0);
    });

    rightButton.addEventListener("click", () => {
        carousel.scrollBy(carousel.clientWidth, 0);
    });

    carousel.addEventListener("scrollend", () => {
        UpdateCarouselButtonsState(carousel, leftButton, rightButton);
    });

    leftButton.disabled = true;
    rightButton.disabled = carousel.scrollLeft == carousel.clientWidth;
}

function UpdateCarouselButtonsState(carousel, leftButton, rightButton) {
    const minScroll = 0;
    leftButton.disabled = carousel.scrollLeft <= minScroll;

    const maxScroll = carousel.scrollWidth - carousel.clientWidth;
    rightButton.disabled = Math.ceil(carousel.scrollLeft) >= maxScroll
}