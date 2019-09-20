enum PageListFormat {
    "1r",
    "I'1r",
    "Roman",
    "Arabic"
};

class PageListGeneratorFactory {
    static createPageListGenerator(format: number): PageListGeneratorBase {
        switch (format) {
            case PageListFormat["1r"]:
                return new RectoVersoPaginationGenerator();
            case PageListFormat["I'1r"]:
                return new RectoVerso2PaginationGenerator();
            case PageListFormat.Roman:
                return new RomanPaginationGenerator();
            case PageListFormat.Arabic:
                return new ArabicPaginationGenerator();
            default:
                return null;
        }
    }
}

abstract class PageListGeneratorBase {
    protected readonly regexTemplate: string;

    protected constructor(regexTemplate: string) {
        this.regexTemplate = regexTemplate;
    }

    protected abstract getPageName(value: number): string;

    protected abstract parseNumber(value: string): number;

    generatePageList(from: string, to: string, doublePage: boolean): string[] {
        const stringList: string[] = [];
        let beginCount = 1;
        let leftPage = "";

        const fromNumber = this.convertToNumber(from);
        const toNumber = this.convertToNumber(to);

        for (let i = fromNumber; i <= toNumber; i++) {
            const pageName = this.getPageName(i);
            if (doublePage) {
                if (beginCount % 2 !== 0) {
                    leftPage = pageName;

                    if (i === toNumber) {
                        stringList.push(pageName);
                    }
                }
                if (beginCount % 2 === 0) {
                    stringList.push(`${leftPage}–${pageName}`);
                }

                beginCount++;
            } else {
                stringList.push(pageName);
            }
        }

        return stringList;
    }

    checkInputValue(value: string) {
        if (value === "")
            return false;

        return (new RegExp(this.regexTemplate)).test(value);
    }

    checkValidPagesLength(from: string, to: string) {
        return this.convertToNumber(from) <= this.convertToNumber(to);
    }

    checkValidDoublePageRange(from: string, to: string) {
        if (!this.checkInputValue(from) || !this.checkInputValue(to)) {
            return false;
        }

        const fromNumber = this.convertToNumber(from);
        const toNumber = this.convertToNumber(to);
        return (toNumber - fromNumber) % 2 === 1;
    }

    protected convertToNumber(value: string): number {
        if (this.checkInputValue(value)) {
            return this.parseNumber(value);
        }

        return NaN;
    }
}

class RomanPaginationGenerator extends PageListGeneratorBase {
    constructor() {
        super("^[MDCLXVI)(]+$");
    }

    getPageName(value: number): string {
        return this.romanise(value);
    }

    protected parseNumber(value: string): number {
        value = value.toUpperCase();

        if (value.indexOf("(") === 0)
            return this.fromBigRoman(value);

        let sum = 0;
        let i = 0;
        let next: number;
        let val: number;

        while (i < value.length) {
            val = this.romanLookup[value.charAt(i++)];
            next = this.romanLookup[value.charAt(i)] || 0;
            if (next - val > 0) val *= -1;
            sum += val;
        }

        if (this.romanise(sum) === value)
            return sum;
        else
            return NaN;
    }

    private romanise(arabic: number): string {
        let roman = "";
        let i: string;
        for (i in this.romanLookup) {
            while (arabic >= this.romanLookup[i]) {
                roman += i;
                arabic -= this.romanLookup[i];
            }
        }
        return roman;
    }

    private fromBigRoman(rn) {
        let sum = 0;
        let splitRoman;
        const rx = /(\(*)([MDCLXVI]+)/g;

        while ((splitRoman = rx.exec(rn)) != null) {
            const exponent = splitRoman[1].length;
            let roman = this.parseNumber(splitRoman[2]);
            if (isNaN(roman)) return NaN;
            if (exponent) roman *= Math.pow(1000, exponent);
            sum += roman;
        }
        return sum;
    }


    private romanLookup = {
        M: 1000,
        CM: 900,
        D: 500,
        CD: 400,
        C: 100,
        XC: 90,
        L: 50,
        XL: 40,
        X: 10,
        IX: 9,
        V: 5,
        IV: 4,
        I: 1
    };
}


class RectoVersoPaginationGenerator extends PageListGeneratorBase {

    constructor() {
        super("^(([1-9]){1}|([0-9]){2,})(r|v)$");
    }

    getPageName(value: number): string {
        const numbericalPart = Math.ceil(value / 2);
        const literalPart = value % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
        return `${numbericalPart}${literalPart}`;
    }

    protected parseNumber(value: string): number {
        let numericValue = Number(value.substr(0, value.length - 2));
        if (value[value.length - 1] === "r") {
            numericValue = - 1;
        }
        return numericValue;
    }
}


class RectoVerso2PaginationGenerator extends PageListGeneratorBase {

    constructor() {
        super("^(I')(([1-9]){1}|([0-9]){2,})(r|v)$");
    }

    getPageName(value: number): string {
        const prefix = "I"; //TODO check how prefix is defined
        const numbericalPart = Math.ceil(value / 2);
        const literalPart = value % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
        return `${prefix}'${numbericalPart}${literalPart}`;
    }

    protected parseNumber(value: string): number {
        let numericValue = Number(value.substr(2, value.length - 2));
        if (value[value.length - 1] === "r") {
            numericValue = - 1;
        }
        return numericValue;
    }
}

class ArabicPaginationGenerator extends PageListGeneratorBase {

    constructor() {
        super("^([1-9]{1}|[0-9]{2,})$");
    }

    getPageName(value: number): string {
        return value.toString();
    }

    protected parseNumber(value: string): number {
        return Number(value);
    }
}