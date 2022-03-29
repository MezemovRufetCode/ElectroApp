$(document).ready(function () {
  //looking
  $("#whishlistPage .grid2 .gridSec").click(function () {
    $("#whishlistPage .item-wrapper").addClass(
      "gridSecondMod col-lg-3 col-md-4 col-6"
    );
    $("#whishlistPage .item").css("display", "flex");
  });
  $("#whishlistPage .grid1 .gridFirst").click(function () {
    $("#whishlistPage .item-wrapper").removeClass(
      "gridSecondMod col-lg-3 col-md-4 col-6"
    );
    $("#whishlistPage .item").css("display", "block");
  });
  //   -------------
  //quantity
  var valueCount;
  document
    .querySelector(".quantity .qty-down")
    .setAttribute("disabled", "disabled");
  document
    .querySelector(".quantity .qty-up")
    .addEventListener("click", function () {
      valueCount = document.getElementById("quantity").value;
      valueCount++;
      document.getElementById("quantity").value = valueCount;
      if (valueCount > 1) {
        document.querySelector(".qty-down").removeAttribute("disabled");
        document.querySelector(".qty-down").classList.remove("disabled");
      }
    });
  document.querySelector(".qty-down").addEventListener("click", function () {
    valueCount = document.getElementById("quantity").value;
    valueCount--;
    document.getElementById("quantity").value = valueCount;
    if (valueCount == 1) {
      document.querySelector(".qty-down").setAttribute("disabled", "disabled");
    }
  });
});
