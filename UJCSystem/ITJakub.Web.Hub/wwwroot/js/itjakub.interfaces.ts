interface IFeedback {
    id: number;
    text: string;
    createDate: string;
    user: IUser;
    filledName: string;
    filledEmail: string;
    category: FeedbackCategoryEnum;
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