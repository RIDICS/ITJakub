$(document).ready(() => {
    if ($(".eupopup").length > 0) {
        $(document).euCookieLawPopup().init({
            //popupTitle: "This website is using cookies.",
            //popupText: "We use them to give you the best experience. If you continue using our website, we'll assume that you are happy to receive all cookies on this website.",
            popupTitle: "Tento web používá cookies.",
            popupText: "Tyto stránky používají soubory cookies s cílem zajistit uživatelům lepší internetový zážitek a dále také zlepšit výkonnost a funkčnost stránek. Používáním těchto webových stránek souhlasíte s použitím souborů cookie.",
            buttonContinueTitle: "Pokračovat",
            buttonLearnmoreTitle: "Zjistit více",
            cookiePolicyUrl: "https://www.google.com/search?q=Eu+cookie+policy&oq=Eu+cookie+policy"
        });
    }
});