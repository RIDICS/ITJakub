class ImageZoom {
    private container: JQuery;
    private image: HTMLImageElement;
    private originalHeight: number;
    private originalWidth: number;
    private imageWidth: number;
    private imageHeight: number;
    private scale: number = 1;
    private bgPositionX: number = 0;
    private bgPositionY: number = 0;

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
        var originalWidth = this.imageWidth;
        var originalHeight = this.imageHeight;
        if (event.deltaY < 0) {
            
            this.imageWidth += this.imageWidth * 0.1;
            this.imageHeight += this.imageHeight * 0.1;
            if (this.imageWidth < this.container.width()) {

                $(this.image)
                    .css("width", this.imageWidth)
                    .css("background-position", "0px 0px");
                this.bgPositionY = 0;
                this.bgPositionX = 0;
            } else {
                this.scale += this.scale * 0.1;
                this.updateBgPosition(originalWidth, originalHeight, event.pageX, event.pageY);
            }

        } else {
            this.imageWidth -= this.imageWidth * 0.1;
            this.imageHeight -= this.imageHeight * 0.1;
            this.imageWidth = Math.max(this.imageWidth, this.originalWidth);
            this.imageHeight = Math.max(this.imageHeight, this.originalHeight);
            
            if (this.scale == 1) {
                $(this.image)
                    .css("width", this.imageWidth)
                    .css("background-position", "0px 0px");
                this.bgPositionY = 0;
                this.bgPositionX = 0;
            } else {
                this.scale -= this.scale * 0.1;
                this.scale = Math.max(this.scale, 1);
                this.updateBgPosition(originalWidth, originalHeight, event.pageX, event.pageY);
            }
        }
        
        $(this.image).css("background-size", `${this.imageWidth}px ${this.imageHeight}px`)

    }
    
    private updateBgPosition(originalWidth: number, originalHeight: number, pageX: number, pageY: number) {
        var imgOffset = $(this.image).offset();
        var cursorPositionX = (pageX - imgOffset.left);
        var cursorPositionY = (pageY - imgOffset.top);

        var cursorRelativeToBgX = cursorPositionX - this.bgPositionX;
        var cursorRelativeToBgY = cursorPositionY - this.bgPositionY;

        var cursorRatioX = cursorRelativeToBgX / originalWidth;
        var cursorRatioY = cursorRelativeToBgY / originalHeight;

        this.bgPositionX = cursorPositionX - (this.imageWidth * cursorRatioX);
        this.bgPositionY = cursorPositionY - (this.imageHeight * cursorRatioY);

        $(this.image).css("background-position", `${this.bgPositionX}px ${this.bgPositionY}px`);
    }
}