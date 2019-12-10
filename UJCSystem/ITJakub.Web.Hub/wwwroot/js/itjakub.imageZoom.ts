class ImageZoom {
    private container: JQuery;
    private image: HTMLImageElement;
    private originalHeight: number;
    private originalWidth: number;
    private imageWidth: number;
    private imageHeight: number;
    private scale: number = 1;

    constructor(image: HTMLImageElement, imageContainer: JQuery) {
        this.image = image;
        this.container = imageContainer;
        this.imageHeight = image.offsetHeight;
        this.imageWidth = image.offsetWidth;
        this.originalHeight = image.offsetHeight;
        this.originalWidth = image.offsetWidth;

        this.initializeZoom();
    }

    private initializeZoom() {
        this.image.style.backgroundImage = 'url("' + this.image.src + '")';
        this.image.style.backgroundRepeat = 'no-repeat';
        this.image.src = "";
        this.image.style.backgroundSize = `${this.originalWidth}px ${this.originalHeight}px`;
        this.image.width = this.originalWidth;
        this.image.height = this.originalHeight;

        this.image.removeEventListener("wheel", this.onWheel);
        this.image.addEventListener("wheel", this.onWheel.bind(this));
    }

    private onWheel(event: WheelEvent) {
        event.preventDefault();

        if (event.deltaY < 0) {
            if (this.imageWidth * 1.1 < this.container.width()) {
                this.imageWidth += this.imageWidth * 0.1;
                this.imageHeight += this.imageHeight * 0.1;
                $(this.image)
                    .css("background-size", `${this.imageWidth}px ${this.imageHeight}px`)
                    .css("width", this.imageWidth)
            }
        } else {
            if (this.scale == 1) {
                this.imageWidth -= this.imageWidth * 0.1;
                this.imageHeight -= this.imageHeight * 0.1;
                this.imageWidth = Math.max(this.imageWidth, this.originalWidth);
                this.imageHeight = Math.max(this.imageHeight, this.originalHeight);
                $(this.image)
                    .css("background-size", `${Math.max(this.imageWidth)}px ${this.imageHeight}px`)
                    .css("width", this.imageWidth)
            }
        }
    }
}