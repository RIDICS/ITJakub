interface IToken {
    id: number;
    text: string;
    description: string;
}

interface ITokenCharacteristic {
    id: number;
    morphologicalCharacteristic: string;
    description: string;
    canonicalFormList: Array<ICanonicalForm>;
}

interface IInverseTokenCharacteristic {
    id: number;
    morphologicalCharacteristic: string;
    description: string;
    token: IToken;
}

interface ICanonicalForm {
    id: number;
    text: string;
    description: string;
    type: CanonicalFormTypeEnum;
    hyperCanonicalForm: IHyperCanonicalForm;
}

interface IInverseCanonicalForm {
    id: number;
    text: string;
    description: string;
    type: CanonicalFormTypeEnum;
    canonicalFormFor: Array<IInverseTokenCharacteristic>;
}

interface IHyperCanonicalForm {
    id: number;
    text: string;
    description: string;
    type: HyperCanonicalFormTypeEnum;
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