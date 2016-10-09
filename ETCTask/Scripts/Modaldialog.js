//Modal dialog
$(function() {
    $.ajaxSetup({ cache: false });

    $("a[data-modal]")
        .on("click",
            function(e) {
                // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
                $(e.target).closest(".btn-group").children(".dropdown-toggle").dropdown("toggle");

                $("#ModalContent")
                    .load(this.href,
                        function() {
                            $("#Modal")
                                .modal({
                                        backdrop: "static",
                                        keyboard: true
                                    },
                                    "show");
                            bindForm(this);
                        });
                return false;
            });
});

function bindForm(dialog) {
    $("form", dialog)
        .submit(function() {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function(result) {
                    if (result.success) {
                        $("#Modal").modal("hide");
                        $("#replacetarget").load(result.url);
                        //  Load data from the server and place the returned HTML into the matched element
                    } else {
                        $("#ModalContent").html(result);
                        bindForm(dialog);
                    }
                }
            });
            return false;
        });
}