
$(document).ready(function () {
  if (typeof (Page_Validators) == 'undefined') return;
  var newValidator = document.createElement('span');
  newValidator.style.display = "none";
  newValidator.id = "contactFieldValidator";
  newValidator.controltovalidate = "primarycontactid";
  newValidator.errormessage = "<a href=\'#primarycontactid_label\' onclick=\'javascript:scrollToAndFocus(\"primarycontactid_label\",\"primarycontactid\"); return false;\'> Contact is a required field. </a>";
  newValidator.initialvalue = "";
  newValidator.evaluationfunction = function () {
    var customerType = $("#customerid_entityname").val();
    if (customerType != "account") return true;
    // only require contact if customer type is account.
    var value = $("#primarycontactid").val();
    if (value == null || value == "") {
      return false;
    } else {
      return true;
    }
  };

  Page_Validators.push(newValidator);

  $("#customerid").change(onCustomerChange);
  $("#customerid").change();
});
  function onCustomerChange() {
    var val = $("#customerid").val();
    var entitlement = $("#entitlementid").parent();
    var product = $("#productid").parent();
    if (val) {
      disableEnable(entitlement, false);
      disableEnable(product, false);
    }
    else {
      disableEnable(entitlement, true);
      disableEnable(product, true);
    }
    updateContactRequired();
  }

  function disableEnable($param, disabled) {
    $param.find('input').each(function () {
      $(this).attr("disabled", disabled);
    });

    $param.find('button').each(function () {
      $(this).attr("disabled", disabled);
    });
  }

  function updateContactRequired() {
    var customerTypeVal = $("#customerid_entityname").val();
    if (customerTypeVal && customerTypeVal == "account") {
      $("#primarycontactid_label").parent().addClass("required");
    }
    else {
      $("#primarycontactid_label").parent().removeClass("required");
    }
  }