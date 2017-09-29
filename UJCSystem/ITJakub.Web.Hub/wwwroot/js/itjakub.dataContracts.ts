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

interface ISaveMetadataResource extends IMetadataResource {
    literaryKindIdList: Array<number>;
    literaryGenreIdList: Array<number>;
    authorIdList: Array<number>;
    responsiblePersonIdList: Array<number>;
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

enum QueryTypeEnum
{
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

interface ICommentSctucture {
    picture: string;
    id: string;
    nested: boolean;
    page: number;
    name: string;
    body: string;
    order: number;
    time: number;
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