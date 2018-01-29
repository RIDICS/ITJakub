enum PageListFormat {
    "1r",
    "I'1r",
    "Roman",
    "Arabic"
};

class PageListGenerator {
    generatePageList(from: number, to: number, style: number, doublePage: boolean): string[
        ] {
        var stringList: string[] = [];
        var beginCount = 1;
        var leftPage = "";
        if (doublePage) {
            to = to * 2;
        }
        switch (style) {
        case PageListFormat["1r"]:
        {
            for (let i = from; i <= to; i++) {
                const numbericalPart = Math.ceil(i / 2);
                const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                const pageName = `${numbericalPart}${literalPart}`;
                if (doublePage) {
                    if (beginCount % 2 !== 0) {
                        leftPage = pageName;
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
            for (let i = from; i <= to; i++) {
                const prefix = "I"; //TODO check how prefix is defined
                const numbericalPart = Math.ceil(i / 2);
                const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                const pageName = `${prefix}'${numbericalPart}${literalPart}`;
                if (doublePage) {
                    if (beginCount % 2 !== 0) {
                        leftPage = pageName;
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
            for (let i = from; i <= to; i++) {
                const pageName = this.romanise(i);
                if (doublePage) {
                    if (beginCount % 2 !== 0) {
                        leftPage = pageName;
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
            for (let i = from; i <= to; i++) {
                const pageName = i.toString();
                if (doublePage) {
                    if (beginCount % 2 !== 0) {
                        leftPage = pageName;
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

    private romanise(arabic: number): string {
        const lookup = {
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
        let roman = "";
        let i: string;
        for (i in lookup) {
            while (arabic >= lookup[i]) {
                roman += i;
                arabic -= lookup[i];
            }
        }
        return roman;
    }
}