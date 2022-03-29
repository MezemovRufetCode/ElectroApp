$(document).ready(function () {
  // Details Page Quantity
  $(document).ready(function () {
    //quantity2
    var valueCount;
    document
      .querySelector(".quantity2 .qty-down2")
      .setAttribute("disabled", "disabled");
    document
      .querySelector(".quantity2 .qty-up2")
      .addEventListener("click", function () {
        valueCount = document.getElementById("quantity2").value;
        valueCount++;
        document.getElementById("quantity2").value = valueCount;
        if (valueCount > 1) {
          document.querySelector(".qty-down2").removeAttribute("disabled");
          document.querySelector(".qty-down2").classList.remove("disabled");
        }
      });
    document.querySelector(".qty-down2").addEventListener("click", function () {
      valueCount = document.getElementById("quantity2").value;
      valueCount--;
      document.getElementById("quantity2").value = valueCount;
      if (valueCount == 1) {
        document
          .querySelector(".qty-down2")
          .setAttribute("disabled", "disabled");
      }
    });
  });
  //   --Ending quantity-

  // product images slider starting
  const imgs = document.querySelectorAll(".img-select a");
  const imgBtns = [...imgs];
  let imgId = 1;
  imgBtns.forEach((imgItem) => {
    imgItem.addEventListener("click", (event) => {
      event.preventDefault();
      imgId = imgItem.dataset.id;
      slideImage();
    });
  });
  function slideImage() {
    const displayWidth = document.querySelector(
      ".img-showcase img:first-child"
    ).clientWidth;
    document.querySelector(".img-showcase").style.transform = `translateX(${
      -(imgId - 1) * displayWidth
    }px)`;
  }
  $("#detailspage .img-item").click(function () {
    $(this)
      .addClass("active-itemlooking")
      .siblings()
      .removeClass("active-itemlooking");
  });

  // ending product images slider

  $("#detailspage .writeopenclose .opclsbtn").click(function () {
    $("#detailspage .writingsection").toggleClass("activeRevWriting");
  });

  //star Rating
  $(".overallstar .my-rating").starRating({
    initialRating: 4,
    strokeColor: "#894A00",
    strokeWidth: 10,
    starSize: 20,
    readOnly: true,
  });
});
