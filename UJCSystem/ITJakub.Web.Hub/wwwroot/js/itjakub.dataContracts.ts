interface IFeedback {
    id: number;
    text: string;
    createDate: string;
    user: IUser;
    filledName: string;
    filledEmail: string;
    category: FeedbackCategoryEnum;
    feedbackType: FeedbackTypeEnum;
    headwordInfo: IFeedbackHeadwordInfo;
}

interface IFeedbackHeadwordInfo {
    headwordId: number;
    headword: string;
    defaultHeadword: string;
    dictionaryName: string;
}

interface IUser {
    id: Number;
    email: string;
    userName: string;
    firstName: string;
    lastName: string;
    createTime: string;
}

interface INewsSyndicationItemContract {
    title: string;
    text: string;
    url: string;
    userEmail: string;
    createDate: string;
    userFirstName: string;
    userLastName: string;
}

interface IFavoriteBaseInfo {
    id: number;
    title: string;
    favoriteLabel: IFavoriteLabel;
    createTime: string;
    favoriteType: FavoriteType;
}

interface IFavoriteLabel {
    id: number;
    name: string;
    color: string;
    isDefault: boolean;
    lastUseTime: string;
}

interface IFavoriteLabelsWithBooksAndCategories {
    id: number;
    name: string;
    color: string;
    bookIdList: number[];
    categoryIdList: number[];
}

interface IFavoriteLabeledBook {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteLabeledCategory {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfo>;
}

interface IFavoriteQuery {
    id: number;
    title: string;
    createTime: string;
    query: string;
    favoriteLabel: IFavoriteLabel;
    bookType?: BookTypeEnum;
    queryType?: QueryTypeEnum;
}

interface IBookPageBookmark {
    id: number;
    pageXmlId: string;
    pagePosition: number;
    title: string;
    favoriteLabel: IFavoriteLabel;
}

interface IOriginalAuthor {
    id: number;
    firstName: string;
    lastName: string;
}

interface IResponsiblePerson {
    id: number;
    firstName: string;
    lastName: string;
}

interface IResponsibleType {
    id: number;
    text: string;
    type: ResponsibleTypeEnum;
}

interface ISaveProjectResponsiblePerson {
    responsiblePersonId: number;
    responsibleTypeId: number;
}

interface IPage {
    id: number;
    versionId: number;
    name: string;
    position: number;
}

interface IPageWithContext extends IPage {
    contextStructure: IKwicStructure;
}

interface IMetadataResource {
    title: string;
    subTitle: string;
    relicAbbreviation: string;
    sourceAbbreviation: string;
    publisherId: number;
    publishPlace: string;
    publishDate: string;
    copyright: string;
    biblText: string;
    originDate: string;
    notBefore: string;
    notAfter: string;
    manuscriptIdno: string;
    manuscriptSettlement: string;
    manuscriptCountry: string;
    manuscriptRepository: string;
    manuscriptExtent: string;
    lastModification?: string;
}

interface IBookContract {
    id: number;
    title: string;
    subTitle: string;
    authors: string;
    relicAbbreviation: string;
    sourceAbbreviation: string;
    publishPlace: string;
    publishDate: string;
    publisherText: string;
    publisherEmail: string;
    copyright: string;
    biblText: string;
    originDate: string;
    notBefore: string;
    notAfter: string;

    manuscriptIdno: string;
    manuscriptSettlement: string;
    manuscriptCountry: string;
    manuscriptRepository: string;
    manuscriptExtent: string;
    manuscriptTitle: string;

    lastModification: string;
}

interface ICorpusSearchResult {
    bookId: number;
    //bookXmlId: string;
    //versionXmlId: string;
    title: string;
    author: string;
    originDate: string;
    relicAbbreviation: string;
    sourceAbbreviation: string;
    notes: Array<string>;
    pageResultContext: IPageWithContext;
    verseResultContext: IVerseResultContext;
    bibleVerseResultContext: IBibleVerseResultContext;
}

interface IKwicStructure {
    before: string;
    match: string;
    after: string;
}

interface IVerseResultContext {
    verseXmlId: string;
    verseName: string;
}

interface IBibleVerseResultContext {
    bibleBook: string;
    bibleChapter: string;
    bibleVerse: string;
}

interface ISaveMetadataResource extends IMetadataResource {
    literaryKindIdList: Array<number>;
    literaryGenreIdList: Array<number>;
    authorIdList: Array<number>;
    projectResponsiblePersonIdList: Array<ISaveProjectResponsiblePerson>;
}

interface IMetadataSaveResult {
    newResourceVersionId: number;
    literaryOriginalText: string;
    lastModificationText: string;
}

enum FavoriteType {
    Unknown = 0,
    Book = 1,
    Category = 2,
    PageBookmark = 3,
    Query = 4,
    BookVersion = 5,
    HeadwordBookmark = 6,
}

enum QueryTypeEnum {
    Search = 0,
    List = 1,
    Reader = 2,
}

enum ResourceType {
    None = 0,
    ResourceGroup = 1,
    ProjectMetadata = 2,
    Chapter = 3,
    Page = 4,
    Headword = 5,
    Text = 6,
    Image = 7,
    Audio = 8,
    Video = 9,
}

enum ResponsibleTypeEnum {
    Unknown = 0,
    Editor = 1,
    Kolace = 2,
}

interface ICommentStructureBase {
    id: number;
    textReferenceId: string;
    text: string;
}

interface ICommentStructureReply extends ICommentStructureBase {
    parentCommentId: number;
}

interface ICommentSctucture extends ICommentStructureBase {
    picture: string;
    nested: boolean;
    textId: number;
    name: string;
    surname: string;
    order: number;
    time: number;
}

interface IParentPage {
    id: number;
    versionId: number;
    name: string;
    position: number;
}

interface ITextProjectPage {
    bookVersionId: number;
    externalId: string;
    id: number;
    parentPage: IParentPage;
    versionId: number;
    versionNumber: number;
}

interface IPageTextBase {
    text: string;
    id: number;
    versionNumber: number;
}

interface IPageText extends IPageTextBase{
    versionId: number;
    externalId: string;
    bookVersionId: number;
}

enum AudioType {
    Unknown = "Unknown",
    Mp3 = "Mp3",
    Ogg = "Ogg",
    Wav = "Wav",
}


//TODO Switch TypeScript to version 2.4 and use enums with String values
//enum ResourceType {
//    None = "None",
//    ResourceGroup = "ResourceGroup",
//    ProjectMetadata = "ProjectMetadata",
//    Chapter = "Chapter",
//    Page = "Page",
//    Headword = "Headword",
//    Text = "Text",
//    Image = "Image",
//    Audio = "Audio",
//    Video = "Video",
//}

//enum ResponsibleTypeEnum {
//    Unknown = "Unknown",
//    Editor = "Editor",
//    Kolace = "Kolace",
//}