$(document).ready(function () {
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
