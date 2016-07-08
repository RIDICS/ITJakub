interface JQuery {
    euCookieLawPopup(): JQueryEuCookieLawPopup
}

declare namespace JQueryEuCookieLawPopup {
    interface Settings {
        cookiePolicyUrl?: string;
        popupPosition?;
        colorStyle?;
        popupTitle?;
        popupText?: string;
        buttonContinueTitle?: string;
        buttonLearnmoreTitle?: string;
        buttonLearnmoreOpenInNewWindow?;
        agreementExpiresInDays?;
        autoAcceptCookiePolicy?;
        htmlMarkup?;
    }
}

declare class JQueryEuCookieLawPopup {
    init(settings: JQueryEuCookieLawPopup.Settings);
}
