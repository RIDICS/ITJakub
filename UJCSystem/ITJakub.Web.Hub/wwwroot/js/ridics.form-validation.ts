$(document.documentElement).ready(() => {
    $("input[type=submit]").click((event) => {
        const targetEl = $(event.target as Node as Element);
        const formEl = targetEl.closest("form");
        if (formEl.length) {
            const validator: JQueryValidation.Validator = formEl.data("validator");
            if (validator) {
                validator.settings.highlight = ((element) => {
                    const jEl = $(element);
                    const formGroup = jEl.closest(".form-group");
                    formGroup.removeClass("has-success").addClass("has-error");
                });
                validator.settings.unhighlight = ((element) => {
                    const jEl = $(element);
                    const formGroup = jEl.closest(".form-group");
                    formGroup.removeClass("has-error").addClass("has-success");
                });
            }
        }
    });
});
