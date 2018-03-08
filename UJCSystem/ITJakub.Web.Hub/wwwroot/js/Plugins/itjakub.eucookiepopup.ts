$(document.documentElement).ready(() => {
    if ($(".eupopup").length > 0) {
        $(document.documentElement).euCookieLawPopup().init({
            //popupTitle: "This website is using cookies.",
            //popupText: "We use them to give you the best experience. If you continue using our website, we'll assume that you are happy to receive all cookies on this website.",
            popupTitle: localization.translate("WebUseCookies", "PluginsJs").value,
            popupText: localization.translate("CookieInfo", "PluginsJs").value,
            buttonContinueTitle: localization.translate("Continue", "PluginsJs").value,
            buttonLearnmoreTitle: localization.translate("LearnMore", "PluginsJs").value,
            cookiePolicyUrl: "https://www.google.com/search?q=Eu+cookie+policy&oq=Eu+cookie+policy"
        });
    }
});