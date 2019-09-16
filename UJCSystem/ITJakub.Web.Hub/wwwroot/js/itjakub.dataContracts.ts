interface IPagedResultArray<T> {
    list: Array<T>;
    totalCount: number;
}

interface IFeedback {
    id: number;
    text: string;
    createTime: string;
    authorUser: IUserDetail;
    authorName: string;
    authorEmail: string;
    feedbackCategory: FeedbackCategoryEnum;
    feedbackType: FeedbackTypeEnum;
    headwordInfo: IHeadwordContract;
    projectInfo: IProject;
}

interface IFeedbackHeadwordInfo {
    headwordId: number;
    headword: string;
    defaultHeadword: string;
    dictionaryName: string;
}

interface IUser {
    id: number;
    userName: string;
    firstName: string;
    lastName: string;
    avatarUrl: string;
}

interface IUserDetail extends IUser {
    email: string;
    createTime: string;
}

interface INewsSyndicationItemContract {
    id: number;
    title: string;
    text: string;
    url: string;
    createTime: string;
    createdByUser: IUser;
}

interface IFavoriteBaseInfo {
    id: number;
    title: string;
    favoriteLabelId?: number;
    createTime?: string;
    favoriteType?: FavoriteType;
}

interface IFavoriteBaseInfoWithLabel extends IFavoriteBaseInfo {
    favoriteLabel: IFavoriteLabel;
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
    projectIdList: number[];
    categoryIdList: number[];
}

interface IFavoriteLabeledBook {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfoWithLabel>;
}

interface IFavoriteLabeledCategory {
    id: number;
    favoriteInfo: Array<IFavoriteBaseInfoWithLabel>;
}

interface IFavoriteQuery extends IFavoriteBaseInfo {
    query: string;
    favoriteLabel: IFavoriteLabel;
    bookType?: BookTypeEnum;
    queryType?: QueryTypeEnum;
}

interface IBookPageBookmark extends IFavoriteBaseInfo {
    pageId: number;
    //pagePosition: number;
    favoriteLabel: IFavoriteLabel;
}

interface IResourceInfo {
    id: number;
    name: string;
    versionNumber: string;
    resourceVersionId: number;
    author: string;
    created: string;
    comment: string;
}

interface IResourceVersion {
    id: number;
    versionNumber: string;
    author: string;
    createDate: string;
    comment: string;
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

interface IProjectResponsiblePersonContract extends IResponsiblePerson{
    responsibleType: IResponsibleType;
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

interface IUpdatePage {
    id?: number;
    name: string;
    position: number;
}

interface IPageWithContext extends IPage {
    contextStructure: IKwicStructure;
}

interface IProject {
    id: number;
    name: string;
}

interface IGetProjectContract extends IProject {
    createdByUser: IUser;
    createTime: string;//DateTime
}

interface IProjectDetailContract extends IGetProjectContract {
    latestMetadata: IMetadataResource;
    pageCount?: number;
    authors: IOriginalAuthor[];
    responsiblePersons: IProjectResponsiblePersonContract[];
}

interface IMetadataResource {
    title: string;
    subTitle: string;
    relicAbbreviation: string;
    sourceAbbreviation: string;
    publisherText: string;
    publishPlace: string;
    publishDate: string;
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

interface ITermContract {
    id: number;
    name: string;
    position: number;
    categoryId: number;
}

interface IChapterHieararchyContract {
    id: number;
    versionId: number;
    name: string;
    position: number;
    beginningPageId: number;
    subChapters: Array<IChapterHieararchyContract>;
}

interface IOnlySaveMetadataResource {
    keywordIdList: Array<number>;
    categoryIdList: Array<number>;
    literaryKindIdList: Array<number>;
    literaryGenreIdList: Array<number>;
    authorIdList: Array<number>;
    projectResponsiblePersonIdList: Array<ISaveProjectResponsiblePerson>;
}

interface ISaveMetadataResource extends IMetadataResource {
    keywordIdList: Array<number>;
    categoryIdList: Array<number>;
    literaryKindIdList: Array<number>;
    literaryGenreIdList: Array<number>;
    authorIdList: Array<number>;
    projectResponsiblePersonIdList: Array<ISaveProjectResponsiblePerson>;
}

interface IGetMetadataResource extends IMetadataResource {
    keywordList?: Array<IKeywordContract>;
    categoryList?: Array<ICategoryContract>;
    literaryKindList?: Array<ILiteraryKindContract>;
    literaryGenreList?: Array<ILiteraryGenreContract>;
    authorIdList?: Array<IOriginalAuthor>;
    responsiblePersonList?: Array<{responsibleType: IResponsibleType , id:number,firestName:string, lastName:string}>;
    literaryOriginalList?: Array<ILiteraryOriginalContract>;
}

interface IMetadataSaveResult {
    newResourceVersionId: number;
    literaryOriginalText: string;
    lastModificationText: string;
}

enum FavoriteType {
    Unknown = "Unknown",
    Project = "Project",
    Category = "Category",
    Page = "Page",
    Query = "Query",
    Snapshot = "Snapshot",
    Headword = "Headword",
}

enum QueryTypeEnum {
    Search = "Search",
    List = "List",
    Reader = "Reader",
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

enum KeyTableEditorType {
    Genre = 0,
    Kind = 1,
    Category = 2,
    ResponsiblePerson = 3,
    ResponsiblePersonType = 4,
    Keyword = 5,
    OriginalAuthor = 6,
    LiteraryOriginal = 7
}

interface IGenreResponseContract {
    id: number,
    name: string;
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

interface ITextWithPage {
    bookVersionId: number;
    id: number;
    parentPage: IPage;
    versionId: number;
    versionNumber: number;
}

interface ICreateTextVersion {
    text: string;
    id: number;
    versionNumber: number;
}

interface ITextWithContent {
    id: number;
    versionId: number;
    versionNumber: number;
    bookVersionId: number;
    text: string;
}

interface ILiteraryGenreContract {
    id: number;
    name: string;
}

interface IKeywordContract {
    id: number;
    name: string;
}

interface ILiteraryOriginalContract {
    id: number;
    name: string;
}

interface ILiteraryKindContract {
    id: number;
    name: string;
}

interface ICategoryContract {
    id: number;
    parentCategoryId?: number;
    externalId: string;
    description: string;
}

interface ICategoryTreeContract {
    text: string;
    id: number;
    children?: ICategoryTreeContract[];
}

interface IEditionNoteContract {
    id: number;
    versionId: number;
    versionNumber: number;
    text: string;
}

interface ICreateEditionNote {
    projectId: number;
    originalVersionId: number;
    content: string;
}

interface IPagedResult<T> {
    totalCount: number;
    list: T[];
}

interface ICorpusSearchViewingPageHistoryEntry {
    compositionResultListStart: number;
    bookId: number;
    hitResultStart: number;
}

interface ICorpusLookupBase {
        start: number;
        count: number;
        contextLength: number;
        snapshotId: number;
        selectedBookIds: number[];
        selectedCategoryIds: number[];
}

interface ICorpusLookupBasicSearch extends ICorpusLookupBase
{
    text: string;
}

interface ICorpusLookupAdvancedSearch extends ICorpusLookupBase {
    json: string;
}

interface ICorpusListLookupContractBase {
sortBooksBy : SortEnum;
sortDirection : SortDirection;
selectedBookIds: number[];
selectedCategoryIds: number[];
}

interface ICorpusListLookupBasicSearchParams extends ICorpusListLookupContractBase{
    text : string;
}

interface ICorpusListLookupAdvancedSearchParams extends ICorpusListLookupContractBase {
    json: string;
}

interface ICorpusListPageLookupBasicSearch extends ICorpusListLookupBasicSearchParams {
    start: number;
    count: number;
}

interface ICorpusListPageLookupAdvancedSearch extends ICorpusListLookupAdvancedSearchParams {
    start: number;
    count: number;
}

interface ISnapshotSearchResultStructure {
    resultCount: number;
    snapshotId: number;
}

interface ICoprusSearchSnapshotResult {
    snapshotList: ISnapshotSearchResultStructure[];
    totalCount: number;
}

interface CorpusSearchTotalResultCountBase {
    selectedSnapshotIds: any[];
    selectedCategoryIds: any[];
}

interface CorpusSearchTotalResultCountBasic extends CorpusSearchTotalResultCountBase{
    text: string;
}

interface CorpusSearchTotalResultCountAdvanced extends CorpusSearchTotalResultCountBase{
    json: string;
}

interface CorpusSearchPagePosition {
    compositionListStart: number;
    bookId: number;
    hitResultStart: number;
}

enum AudioType {
    Unknown = "Unknown",
    Mp3 = "Mp3",
    Ogg = "Ogg",
    Wav = "Wav",
}

enum TextFormatEnumContract {
    Raw = 0,
    Html = 1
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

interface IForumViewModel {
    name: string;
    url: string;
}