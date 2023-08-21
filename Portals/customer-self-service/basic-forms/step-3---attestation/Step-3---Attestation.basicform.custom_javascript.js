if (window.jQuery) {
  (function ($) {
    $(document).ready(function () {
        $("#UpdateButton").attr('disabled','disabled');
        $("#UpdateButton").css('cursor','not-allowed');
        $('#cs_attestation_1').on('change', function () {
            $("#UpdateButton").removeAttr('disabled');            
            $("#UpdateButton").css('cursor','default');        
        });

    })
  })(window.jQuery)
}