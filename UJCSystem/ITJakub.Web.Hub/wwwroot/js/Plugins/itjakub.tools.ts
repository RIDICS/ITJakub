enum HttpStatusCode {
    Success = 200,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    InternalServerError = 500,
}

interface IDictionary<T> {
    [key: number]: T;
    [key: string]: T;
}

class DictionaryWrapper<T> {
    private data: IDictionary<T>;

    constructor() {
        this.data = {}
    }

    public add(key: (string|number), value: T) {
        this.setValue(key, value);
    }

    public setValue(key: (string|number), value: T) {
        this.data[key] = value;
    }

    public get(key: (string|number)): T {
        return this.data[key];
    }
}

class Set {
    private data: Object;

    constructor() {
        this.data = {};
    }

    public add(key: string|number) {
        this.data[key] = true;
    }

    public addAll(keyList: Array<string|number>) {
        for (var i = 0; i < keyList.length; i++) {
            this.add(keyList[i]);
        }
    }

    public contains(key: string|number): boolean {
        return this.data[key] === true;
    }

    public remove(key: string|number) {
        delete this.data[key];
    }

    public clear() {
        this.data = {};
    }

    public isEmpty(): boolean {
        return $.isEmptyObject(this.data);
    }
}

class TimeSpan {
    private hours = 0;
    private minutes = 0;
    private seconds = 0;
    private milliseconds = 0;

    constructor(data: string) {
        var tokens = data.split(":");
        this.hours = Number(tokens[0]);

        if (tokens.length >= 2) {
            this.minutes = Number(tokens[1]);
        }
        if (tokens.length >= 3) {
            var secTokens = tokens[2].split(".");
            this.seconds = Number(secTokens[0]);

            if (secTokens.length > 1) {
                this.milliseconds = Number(secTokens[1]);
            }
        }
    }

    getHours(): number {
        return this.hours;
    }
    getMinutes(): number {
        return this.minutes;
    }
    getSeconds(): number {
        return this.seconds;
    }
    getMilliseconds(): number {
        return this.milliseconds;
    }

    toShortString(): string {
        if (this.hours > 0) {
            return this.hours.toString() + ":" + this.fillLeadingZero(this.minutes) + ":" + this.fillLeadingZero(this.seconds);
        } else {
            return this.minutes.toString() + ":" + this.fillLeadingZero(this.seconds);
        }
    }

    private fillLeadingZero(timeValue: number) {
        var secondsString = timeValue.toString();
        if (secondsString.length === 1) {
            secondsString = `0${secondsString}`;
        }
        return secondsString;
    }
}

class Guid {
    public static generate(): string {
        return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, c => {
            var r = Math.random() * 16 | 0, v = c === "x" ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

class HexColor {
    private hexColor: string;
    private isValidHexColorValue: boolean;
    private red: number;
    private green: number;
    private blue: number;

    constructor(hexColor: string) {
        this.hexColor = hexColor;
        this.isValidHexColorValue = hexColor.length === 7 && hexColor.charAt(0) === "#";

        if (this.isValidHexColorValue) {
            this.red = parseInt(hexColor.substr(1, 2), 16);
            this.green = parseInt(hexColor.substr(3, 2), 16);
            this.blue = parseInt(hexColor.substr(5, 2), 16);
        }
    }

    public isValidHexColor(): boolean {
        return this.isValidHexColorValue;
    }

    public getColor(): string {
        return this.hexColor;
    }

    public getRed(): number {
        return this.red;
    }

    public getBlue(): number {
        return this.blue;
    }

    public getGreen(): number {
        return this.green;
    }

    public getColorBrightness(): number {
        return ((this.red * 299) + (this.green * 587) + (this.blue * 114)) / 1000;
    }

    public getIncreasedHexColor(percent: number): string {
        return '#' +
            ((0 | (1 << 8) + this.red + (256 - this.red) * percent / 100).toString(16)).substr(1) +
            ((0 | (1 << 8) + this.green + (256 - this.green) * percent / 100).toString(16)).substr(1) +
            ((0 | (1 << 8) + this.blue + (256 - this.blue) * percent / 100).toString(16)).substr(1);
    }

    public getDecreasedHexColor(percent: number): string {
        return '#' +
            ((0 | (1 << 8) + this.red - this.red * percent / 100).toString(16)).substr(1) +
            ((0 | (1 << 8) + this.green - this.green * percent / 100).toString(16)).substr(1) +
            ((0 | (1 << 8) + this.blue - this.blue * percent / 100).toString(16)).substr(1);
    }
}