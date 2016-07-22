interface IToken {
    Id: number;
    Text: string;
    Description: string;
}

interface ITokenCharacteristic {
    Id: number;
    MorphologicalCharacteristic: string;
    Description: string;
    CanonicalFormList: Array<ICanonicalForm>;
}

interface IInverseTokenCharacteristic {
    Id: number;
    MorphologicalCharacteristic: string;
    Description: string;
    Token: IToken;
}

interface ICanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: CanonicalFormTypeEnum;
    HyperCanonicalForm: IHyperCanonicalForm;
}

interface IInverseCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: CanonicalFormTypeEnum;
    CanonicalFormFor: Array<IInverseTokenCharacteristic>;
}

interface IHyperCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: HyperCanonicalFormTypeEnum;
}

enum CanonicalFormTypeEnum {
    Lemma = 0,
    Stemma = 1,
    LemmaOld = 2,
    StemmaOld = 3,
}

enum HyperCanonicalFormTypeEnum {
    HyperLemma = 0,
    HyperStemma = 1,
}