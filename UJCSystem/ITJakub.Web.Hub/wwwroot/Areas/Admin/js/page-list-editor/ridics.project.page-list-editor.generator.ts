enum PageListFormat {
    "1r",
    "I'1r",
    "Roman",
    "Arabic"
};

class PageListGenerator {
    generatePageList(from: number, to: number, style: number, fc: boolean, fs: boolean): string[] {
        var stringList: string[] = [];
        if (fc) {
            stringList.push("FC");
        }
        if (fs) {
            stringList.push("FS");
        }
        switch (style) {
        case PageListFormat["1r"]:
        {
            for (let i = from; i <= to; i++) {
                const numbericalPart = Math.ceil(i / 2);
                const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                stringList.push(`${numbericalPart}${literalPart}`);
            }
            break;
        }
        case PageListFormat["I'1r"]:
        {
            for (let i = from; i <= to; i++) {
                const prefix = "I"; //TODO check how prefix is defined
                const numbericalPart = Math.ceil(i / 2);
                const literalPart = i % 2 === 0 ? "v" : "r"; //TODO check whether v/r depict even/odd
                stringList.push(`${prefix}'${numbericalPart}${literalPart}`);
            }
            break;
        }
        case PageListFormat.Roman:
        {
            for (let i = from; i <= to; i++) {
                stringList.push(this.romanise(i));
            }
            break;
        }
        case PageListFormat.Arabic:
        {
            for (let i = from; i <= to; i++) {
                stringList.push(i.toString());
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