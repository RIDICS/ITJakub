enum PageListFormat {
    "1r",
    "I'1r",
    "Roman",
    "Arabic"
};

class PageListGenerator {
    private readonly regex1rTemplate = "^(([1-9]){1}|([0-9]){2,})(r|v)$";
    private readonly regexI1rTemplate = `^(I')(([1-9]){1}|([0-9]){2,})(r|v)$`;
    private readonly regexRomanTemplate = "^[MDCLXVI)(]+$";
    private readonly regexArabicTemplate = "^([1-9]{1}|[0-9]{2,})$";

    generatePageList(from: string, to: string, style: number, doublePage: boolean): string[] {
        var stringList: string[] = [];
        var beginCount = 1;
        var leftPage = "";
       
        const fromNumber = this.convertToNumber(from, style);
        const toNumber = this.convertToNumber(to, style);
        
        switch (style) {
            case PageListFormat["1r"]:
                {
                    for (let i = fromNumber; i <= toNumber; i++) {
                        const numbericalPart = Math.ceil(i / 2);
                        const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                        const pageName = `${numbericalPart}${literalPart}`;
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
                    break;
                }
            case PageListFormat["I'1r"]:
                {
                    for (let i = fromNumber; i <= toNumber; i++) {
                        const prefix = "I"; //TODO check how prefix is defined
                        const numbericalPart = Math.ceil(i / 2);
                        const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                        const pageName = `${prefix}'${numbericalPart}${literalPart}`;
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
                    break;
                }
            case PageListFormat.Roman:
                {
                    for (let i = fromNumber; i <= toNumber; i++) {
                        const pageName = this.romanise(i);
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
                    break;
                }
            case PageListFormat.Arabic:
                {
                    for (let i = fromNumber; i <= toNumber; i++) {
                        const pageName = i.toString();
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
                    break;
                }
            default:
                {
                    alert("Invalid format");
                    break;
                }
        }
        return stringList;
    }

    checkInputValue(value: string, style: number) {
        if (value === "")
            return false;

        let regex: RegExp;
        switch (style) {
            case PageListFormat["1r"]:
                regex = new RegExp(this.regex1rTemplate);
                break;
            case PageListFormat["I'1r"]:
                regex = new RegExp(this.regexI1rTemplate);
                break;
            case PageListFormat.Roman:
                regex = new RegExp(this.regexRomanTemplate);
                break;
            case PageListFormat.Arabic:
                regex = new RegExp(this.regexArabicTemplate);
                break;
            default:
                return false;
        }
        return regex.test(value);
    }

    checkValidPagesLength(from: string, to: string, style: number) {
        return this.convertToNumber(from, style) <= this.convertToNumber(to, style);
    }

    checkValidDoublePageRange(from: string, to: string, style: number) {
        if (!this.checkInputValue(from, style) || !this.checkInputValue(to, style)) {
            return false;
        }

        switch (style) {
            case PageListFormat["1r"]:
            {
                const first = from[from.length - 1];
                const last = to[to.length - 1];
                return first !== last;
            }
            case PageListFormat["I'1r"]:
            {
                const first = from[from.length - 1];
                const last = to[to.length - 1];
                return first !== last;
            }
            case PageListFormat.Roman:
            case PageListFormat.Arabic:
            {
                const fromNumber = this.convertToNumber(from, style);
                const toNumber = this.convertToNumber(to, style);
                return (toNumber - fromNumber) % 2 === 1;
            }
        }

        return false;
    }

    private convertToNumber(value: string, style: number): number {
        if (this.checkInputValue(value, style)) {
            switch (style) {
            case PageListFormat["1r"]:
            {
                let numericValue = Number(value.substr(0, value.length - 2));
                if (value[value.length - 1] === "r") {
                    numericValue = - 1;
                }
                return numericValue;
            }
            case PageListFormat["I'1r"]:
            {
                let numericValue = Number(value.substr(2, value.length - 2));
                if (value[value.length - 1] === "r") {
                    numericValue = - 1;
                }
                return numericValue;
            }
            case PageListFormat.Roman:
                return this.fromRoman(value);
            case PageListFormat.Arabic:
                return Number(value);
            default:
                return NaN;
            }
        }
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
            let roman = this.fromRoman(splitRoman[2]);
            if (isNaN(roman)) return NaN;
            if (exponent) roman *= Math.pow(1000, exponent);
            sum += roman;
        }
        return sum;
    }

    private fromRoman(roman): number {
        roman = roman.toUpperCase();
        const reg = new RegExp(this.regexRomanTemplate);
        if (reg.test(roman)) {
            if (roman.indexOf("(") === 0)
                return this.fromBigRoman(roman);

            let sum = 0;
            let i = 0;
            let next: number;
            let val: number;

            while (i < roman.length) {
                val = this.romanLookup[roman.charAt(i++)];
                next = this.romanLookup[roman.charAt(i)] || 0;
                if (next - val > 0) val *= -1;
                sum += val;
            }

            if (this.romanise(sum) === roman)
                return sum;
        }
        return NaN;
    };

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