function InitializeHomePage(popularPodcasts, newPodcasts)
{
    InitializePodcastsCarousel(popularPodcasts);
    InitializePodcastsCarousel(newPodcasts);   
}

function InitializePodcastsCarousel(carousel) {
    var carouselContent = carousel.getElementsByClassName("carouselContent")[0];

    carouselContent.addEventListener("scrollend", () => {
        UpdateCarouselButtonsVisibility(carousel, carouselContent);
    });

    carousel.dataset.atStart = true;
    carousel.dataset.atEnd = carouselContent.scrollLeft == carouselContent.clientWidth;
}

function UpdateCarouselButtonsVisibility(carousel, carouselContent) {
    const minScroll = 0;
    carousel.dataset.atStart = carouselContent.scrollLeft <= minScroll;

    const maxScroll = carouselContent.scrollWidth - carouselContent.clientWidth;
    carousel.dataset.atEnd = Math.ceil(carouselContent.scrollLeft) >= maxScroll;
}