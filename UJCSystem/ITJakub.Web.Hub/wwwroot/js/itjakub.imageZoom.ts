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
    private previousDragEvent: MouseEvent;
    private config: IImageZoomConfig;

    constructor(image: HTMLImageElement, imageContainer: JQuery, config?: IImageZoomConfig) {
        this.image = image;
        this.container = imageContainer;
        this.imageHeight = image.offsetHeight;
        this.imageWidth = image.offsetWidth;
        this.originalHeight = image.offsetHeight;
        this.originalWidth = image.offsetWidth;

        if (config) {
            this.config = config;
        } else {
            this.config = {
                maxScale: 2.5,
                scaleStep: 0.1,
                maxImageWidth: 1150
            };
        }

        this.imageToBackground();

        this.initializeZoom();
        this.initializeDrag();
    }

    private imageToBackground() {
        this.image.style.backgroundImage = 'url("' + this.image.src + '")';
        this.image.style.backgroundRepeat = 'no-repeat';

        var canvas = document.createElement("canvas");
        canvas.width = this.imageWidth;
        canvas.height = this.imageHeight;
        this.image.src = canvas.toDataURL();

        this.image.style.backgroundSize = `${this.originalWidth}px ${this.originalHeight}px`;
    }

    private initializeZoom() {

        this.image.width = this.originalWidth;
        this.image.height = this.originalHeight;
        var onWheel = this.onWheel.bind(this);
        this.image.removeEventListener("wheel", onWheel);
        this.image.addEventListener("wheel", onWheel);
    }

    private onWheel(event: WheelEvent) {
        event.preventDefault();
        var originalWidth = this.imageWidth;
        var originalHeight = this.imageHeight;
        if (event.deltaY < 0) {

            var newWidth = this.imageWidth + this.imageWidth * this.config.scaleStep;
            var newHeight = this.imageHeight + this.imageHeight * this.config.scaleStep;
            if (newWidth < this.container.width() && newWidth < this.config.maxImageWidth) {
                this.imageHeight = newHeight;
                this.imageWidth = newWidth;
                $(this.image)
                    .css("width", this.imageWidth)
                    .css("background-position", "0px 0px");
                this.bgPositionY = 0;
                this.bgPositionX = 0;
                this.scale = 1;
            } else {
                if (this.scale > this.config.maxScale) {
                    return;
                }
                this.imageHeight = newHeight;
                this.imageWidth = newWidth;
                this.scale += this.scale * this.config.scaleStep;
                this.updateBgPositionRelative(originalWidth, originalHeight, event.pageX, event.pageY);
            }

        } else {
            this.imageWidth -= this.imageWidth * this.config.scaleStep;
            this.imageHeight -= this.imageHeight * this.config.scaleStep;
            this.imageWidth = Math.max(this.imageWidth, this.originalWidth);
            this.imageHeight = Math.max(this.imageHeight, this.originalHeight);

            if (this.scale <= 1) {
                $(this.image)
                    .css("width", this.imageWidth)
                    .css("background-position", "0px 0px");
                this.bgPositionY = 0;
                this.bgPositionX = 0;
            } else {
                this.scale -= this.scale * this.config.scaleStep;
                this.scale = Math.max(this.scale, 1);
                this.updateBgPositionRelative(originalWidth, originalHeight, event.pageX, event.pageY);
            }
        }

        $(this.image).css("background-size", `${this.imageWidth}px ${this.imageHeight}px`)

    }

    private updateBgPositionRelative(originalWidth: number, originalHeight: number, pageX: number, pageY: number) {
        var imgOffset = $(this.image).offset();
        var cursorPositionX = (pageX - imgOffset.left);
        var cursorPositionY = (pageY - imgOffset.top);

        var cursorRelativeToBgX = cursorPositionX - this.bgPositionX;
        var cursorRelativeToBgY = cursorPositionY - this.bgPositionY;

        var cursorRatioX = cursorRelativeToBgX / originalWidth;
        var cursorRatioY = cursorRelativeToBgY / originalHeight;

        this.bgPositionX = cursorPositionX - (this.imageWidth * cursorRatioX);
        this.bgPositionY = cursorPositionY - (this.imageHeight * cursorRatioY);

        this.updateBgPosition()
    }

    private updateBgPosition() {
        var imageElWidth = $(this.image).width();
        if (this.bgPositionX > 0) {
            this.bgPositionX = 0;
        } else if (this.bgPositionX < imageElWidth - this.imageWidth) {
            this.bgPositionX = imageElWidth - this.imageWidth;
        }

        if (this.bgPositionY > 0) {
            this.bgPositionY = 0;
        } else if (this.bgPositionY < this.originalHeight - this.imageHeight) {
            this.bgPositionY = this.originalHeight - this.imageHeight;
        }

        $(this.image).css("background-position", `${this.bgPositionX}px ${this.bgPositionY}px`);

    }

    private initializeDrag() {
        var $image = $(this.image);
        $image.off("mousedown");
        $image.off("mouseup mouseleave");
        $image.on("mousedown", this.onMouseDown.bind(this));
        $image.on("mouseup mouseleave", this.onMouseUp.bind(this));
    }

    private onMouseDown(event: MouseEvent) {
        event.preventDefault();
        this.previousDragEvent = event;
        $(this.image).on("mousemove", this.onDrag.bind(this));
    }

    private onMouseUp(event: MouseEvent) {
        event.preventDefault();
        this.previousDragEvent = event;
        $(this.image).off("mousemove");
    }

    private onDrag(event: MouseEvent) {
        event.preventDefault();
        this.bgPositionX += (event.pageX - this.previousDragEvent.pageX);
        this.bgPositionY += (event.pageY - this.previousDragEvent.pageY);
        this.previousDragEvent = event;
        this.updateBgPosition();
    }
}

interface IImageZoomConfig {
    maxScale: number;
    maxImageWidth: number;
    scaleStep: number;
}