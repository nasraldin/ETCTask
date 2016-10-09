(function() {
    $(function() {
        var loadCount = 1;
        $("#btn-more")
            .on("click",
                function(e) {
                    e.preventDefault();
                    var url = "/Home/LoadMoreProduct/";
                    $.ajax({
                        url: url,
                        data: { size: loadCount * 3 },
                        cache: false,
                        type: "POST",
                        success: function(data) {
                            if (data.length !== 0) {
                                $(data.ModelString).insertBefore("#getD").hide().fadeIn(200);
                            }

                            var ajaxModelCount = data.ModelCount - (loadCount * 3);
                            if (ajaxModelCount <= 0) {
                                $("#btn-more").hide().fadeOut(200);
                            }
                        }
                    });
                    loadCount = loadCount + 1;
                });
    });
})();