(function () {
  "use strict";

  document.head.insertAdjacentHTML("beforeend", "<style>.admin-page{max-width:1180px}.admin-stats{display:grid;grid-template-columns:repeat(5,1fr);gap:12px;margin:28px 0}.admin-stats article{background:var(--mint,#d5eadf);padding:20px;display:grid;gap:5px}.admin-stats strong{font-size:32px;color:var(--plum,#573c59)}.admin-actions{display:grid;grid-template-columns:repeat(3,1fr);gap:18px;align-items:start}.admin-actions .form-card{display:grid;gap:7px}.admin-section{margin-top:34px}.admin-section-heading{display:flex;justify-content:space-between;align-items:baseline;border-bottom:2px solid var(--plum,#573c59);margin-bottom:12px}.admin-hotel{background:#fffdf9;padding:18px;margin:12px 0;border:1px solid #ded2d7}.admin-hotel-heading{display:flex;justify-content:space-between;gap:14px;align-items:start}.admin-hotel-heading h3{margin:0}.admin-hotel-heading p{margin:5px 0}.admin-room-list{display:grid;gap:7px}.admin-room{display:flex;justify-content:space-between;gap:12px;padding:9px 0;border-top:1px solid #eadfe3}.admin-table-wrap{overflow-x:auto;background:#fffdf9}.admin-table{width:100%;border-collapse:collapse}.admin-table th,.admin-table td{padding:11px;border-bottom:1px solid #eadfe3;text-align:left;white-space:nowrap}.dark .admin-hotel,.dark .admin-table-wrap{background:#26212a;color:#d9ced5}.dark .admin-room{border-color:#514553}.dark .admin-stats article{background:#304238}.dark .admin-stats strong{color:#d9ced5}@media(max-width:900px){.admin-stats{grid-template-columns:repeat(3,1fr)}.admin-actions{grid-template-columns:1fr 1fr}}@media(max-width:600px){.admin-stats{grid-template-columns:repeat(2,1fr)}.admin-actions{grid-template-columns:1fr}.admin-hotel-heading,.admin-room{display:grid;grid-template-columns:1fr}.admin-table{font-size:13px}}</style>");

  function role() {
    try {
      var token = localStorage.getItem("vacayToken");
      if (!token) return "";
      var payload = JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
      return payload.role ||
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"] || "";
    } catch (_) {
      return "";
    }
  }

  function escapeHtml(value) {
    return String(value == null ? "" : value).replace(/[&<>"]/g, function (ch) {
      return { "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;" }[ch];
    });
  }

  function date(value) {
    return value ? new Date(value).toLocaleDateString() : "—";
  }

  function dashboard() {
    if (role() !== "Admin") {
      location.hash = "login";
      return;
    }
    app.innerHTML = "<section class=\"page admin-page\"><p class=\"tag\">Administration</p><h1>Vacay admin dashboard</h1><p class=\"lead\">Manage the catalogue and review activity from one protected workspace.</p><div id=\"admin-content\"><p>Loading administration data…</p></div></section>";
    loadDashboard();
  }

  async function loadDashboard() {
    var root = document.querySelector("#admin-content");
    if (!root) return;
    try {
      var results = await Promise.all([api("/api/admin/dashboard"), api("/api/hotels")]);
      var summary = results[0];
      var hotels = results[1];
      var hotelRooms = await Promise.all(hotels.map(async function (hotel) {
        return Object.assign({}, hotel, { rooms: await api("/api/hotels/" + hotel.id + "/rooms") });
      }));
      var options = hotels.map(function (hotel) {
        return "<option value=\"" + hotel.id + "\">" + escapeHtml(hotel.name) + " — " + escapeHtml(hotel.city) + "</option>";
      }).join("");
      var cityOptions = cities.map(function (cityName) { return "<option>" + escapeHtml(cityName) + "</option>"; }).join("");

      var hotelCards = hotelRooms.map(function (hotel) {
        var rooms = hotel.rooms.map(function (room) {
          return "<div class=\"admin-room\"><span><strong>" + escapeHtml(room.name) + "</strong> · $" + room.price + "/night · " + room.capacity + " guests · " + (room.breakfastIncluded ? "breakfast included" : "room only") + "</span><button type=\"button\" data-admin-delete-room=\"" + room.id + "\" data-hotel-id=\"" + hotel.id + "\">Delete room</button></div>";
        }).join("");
        return "<article class=\"admin-hotel\"><div class=\"admin-hotel-heading\"><div><h3>" + escapeHtml(hotel.name) + "</h3><p>" + escapeHtml(hotel.city) + " · " + escapeHtml(hotel.address) + " · " + "★".repeat(hotel.rating) + "</p></div><button type=\"button\" data-admin-delete-hotel=\"" + hotel.id + "\">Delete hotel</button></div><div class=\"admin-room-list\">" + (rooms || "<p>No rooms yet.</p>") + "</div></article>";
      }).join("");

      var managers = summary.managers.map(function (manager) {
        return "<tr><td>" + escapeHtml(manager.firstName) + " " + escapeHtml(manager.lastName) + "</td><td>" + escapeHtml(manager.email) + "</td><td>" + escapeHtml(manager.hotel) + "</td><td><button data-admin-delete-manager=\"" + manager.id + "\">Delete</button></td></tr>";
      }).join("") || "<tr><td colspan=\"4\">No managers yet.</td></tr>";
      var reservations = summary.reservations.map(function (reservation) {
        return "<tr><td>" + escapeHtml(reservation.guest.firstName) + " " + escapeHtml(reservation.guest.lastName) + "</td><td>" + date(reservation.checkInDate) + " → " + date(reservation.checkOutDate) + "</td><td>" + reservation.rooms.map(function (room) { return escapeHtml(room.name); }).join(", ") + "</td><td>" + reservation.rooms.map(function (room) { return escapeHtml(room.hotel); }).join(", ") + "</td></tr>";
      }).join("") || "<tr><td colspan=\"4\">No reservations yet.</td></tr>";
      var rentals = summary.rentals.map(function (rental) {
        return "<tr><td>" + escapeHtml(rental.guest.firstName) + " " + escapeHtml(rental.guest.lastName) + "</td><td>" + escapeHtml(rental.car.brand) + " " + escapeHtml(rental.car.model) + "</td><td>" + date(rental.pickupDate) + " → " + date(rental.dropoffDate) + "</td><td>" + escapeHtml(rental.pickupLocation) + "</td></tr>";
      }).join("") || "<tr><td colspan=\"4\">No rentals yet.</td></tr>";

      root.innerHTML =
        "<div class=\"admin-stats\">" +
          "<article><strong>" + summary.totalHotels + "</strong><span>Hotels</span></article>" +
          "<article><strong>" + summary.totalRooms + "</strong><span>Rooms</span></article>" +
          "<article><strong>" + summary.totalGuests + "</strong><span>Guests</span></article>" +
          "<article><strong>" + summary.activeReservations + "</strong><span>Active stays</span></article>" +
          "<article><strong>" + summary.totalCarRentals + "</strong><span>Car rentals</span></article>" +
        "</div>" +
        "<div class=\"admin-actions\">" +
          "<form id=\"admin-hotel-form\" class=\"form-card\"><h2>Add hotel</h2><label>Name</label><input name=\"name\" required><label>Rating (1–5)</label><input name=\"rating\" type=\"number\" min=\"1\" max=\"5\" value=\"4\" required><label>Country</label><input name=\"country\" value=\"Georgia\" required><label>City</label><select name=\"city\">" + cityOptions + "</select><label>Address</label><input name=\"address\" required><button class=\"primary\">Create hotel</button><div data-admin-message=\"hotel\"></div></form>" +
          "<form id=\"admin-room-form\" class=\"form-card\"><h2>Add room</h2><label>Hotel</label><select name=\"hotelId\" required>" + options + "</select><label>Room name</label><input name=\"name\" placeholder=\"Deluxe King\" required><label>Price per night</label><input name=\"price\" type=\"number\" min=\"1\" step=\"0.01\" required><label>Capacity</label><input name=\"capacity\" type=\"number\" min=\"1\" value=\"2\" required><label>Size (m²)</label><input name=\"sizeSquareMeters\" type=\"number\" min=\"1\" value=\"25\" required><label><input name=\"breakfastIncluded\" type=\"checkbox\"> Breakfast included</label><button class=\"primary\">Create room</button><div data-admin-message=\"room\"></div></form>" +
          "<form id=\"admin-manager-form\" class=\"form-card\"><h2>Add manager</h2><label>Hotel</label><select name=\"hotelId\" required>" + options + "</select><label>First name</label><input name=\"firstName\" required><label>Last name</label><input name=\"lastName\" required><label>Personal number</label><input name=\"personalNumber\" required><label>Email</label><input name=\"email\" type=\"email\" required><label>Phone number</label><input name=\"phoneNumber\" required><label>Temporary password</label><input name=\"password\" type=\"password\" minlength=\"8\" required><button class=\"primary\">Create manager</button><div data-admin-message=\"manager\"></div></form>" +
        "</div>" +
        "<section class=\"admin-section\"><div class=\"admin-section-heading\"><h2>Hotels and rooms</h2><span>" + hotelRooms.length + " properties</span></div>" + hotelCards + "</section>" +
        "<section class=\"admin-section\"><div class=\"admin-section-heading\"><h2>Managers</h2><span>" + summary.managers.length + " accounts</span></div><div class=\"admin-table-wrap\"><table class=\"admin-table\"><thead><tr><th>Name</th><th>Email</th><th>Hotel</th><th>Action</th></tr></thead><tbody>" + managers + "</tbody></table></div></section>" +
        "<section class=\"admin-section\"><div class=\"admin-section-heading\"><h2>Recent reservations</h2><span>Latest 30</span></div><div class=\"admin-table-wrap\"><table class=\"admin-table\"><thead><tr><th>Guest</th><th>Dates</th><th>Room</th><th>Property</th></tr></thead><tbody>" + reservations + "</tbody></table></div></section>" +
        "<section class=\"admin-section\"><div class=\"admin-section-heading\"><h2>Recent car rentals</h2><span>Latest 30</span></div><div class=\"admin-table-wrap\"><table class=\"admin-table\"><thead><tr><th>Guest</th><th>Car</th><th>Dates</th><th>Pick-up</th></tr></thead><tbody>" + rentals + "</tbody></table></div></section>";
      bindForms();
    } catch (error) {
      root.innerHTML = "<div class=\"error\">" + escapeHtml(error.message) + "</div><p>Only an Admin account can open this dashboard.</p>";
    }
  }

  function values(form) {
    var result = Object.fromEntries(new FormData(form));
    result.rating = Number(result.rating);
    result.price = Number(result.price);
    result.capacity = Number(result.capacity);
    result.sizeSquareMeters = Number(result.sizeSquareMeters);
    result.breakfastIncluded = form.elements.breakfastIncluded ? form.elements.breakfastIncluded.checked : false;
    return result;
  }

  function showError(name, error) {
    var slot = document.querySelector("[data-admin-message=\"" + name + "\"]");
    if (slot) slot.innerHTML = "<div class=\"error\">" + escapeHtml(error.message) + "</div>";
  }

  function showDeleteMessage(message, isError) {
    var root = document.querySelector("#admin-content");
    if (!root) return;
    var existing = root.querySelector("[data-admin-delete-message]");
    if (existing) existing.remove();
    root.insertAdjacentHTML("afterbegin", "<div data-admin-delete-message class=\"" + (isError ? "error" : "notice") + " role=\"status\">" + escapeHtml(message) + "</div>");
  }

  function bindForms() {
    var hotel = document.querySelector("#admin-hotel-form");
    hotel.onsubmit = async function (event) {
      event.preventDefault();
      try {
        await api("/api/hotels", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(values(event.target)) });
        loadDashboard();
      } catch (error) { showError("hotel", error); }
    };

    var room = document.querySelector("#admin-room-form");
    room.onsubmit = async function (event) {
      event.preventDefault();
      var data = values(event.target);
      var hotelId = data.hotelId;
      delete data.hotelId;
      try {
        await api("/api/hotels/" + hotelId + "/rooms", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) });
        event.target.reset();
        await loadDashboard();
        var success = document.querySelector('[data-admin-message="room"]');
        if (success) success.innerHTML = "<div class=\"notice\" role=\"status\">Room created successfully and published live.</div>";
      } catch (error) { showError("room", error); }
    };

    var manager = document.querySelector("#admin-manager-form");
    manager.onsubmit = async function (event) {
      event.preventDefault();
      var data = Object.fromEntries(new FormData(event.target));
      var hotelId = data.hotelId;
      delete data.hotelId;
      try {
        await api("/api/hotels/" + hotelId + "/managers", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) });
        loadDashboard();
      } catch (error) { showError("manager", error); }
    };

    document.querySelectorAll("[data-admin-delete-hotel]").forEach(function (button) {
      button.onclick = async function () {
        if (!confirm("Delete this hotel and all of its rooms? Active or future reservations must be cancelled first.")) return;
        button.disabled = true;
        try { await api("/api/hotels/" + button.dataset.adminDeleteHotel, { method: "DELETE" }); await loadDashboard(); showDeleteMessage("Hotel and its rooms were deleted successfully.", false); } catch (error) { showDeleteMessage(error.message, true); } finally { button.disabled = false; }
      };
    });
    document.querySelectorAll("[data-admin-delete-room]").forEach(function (button) {
      button.onclick = async function () {
        if (!confirm("Delete this room?")) return;
        button.disabled = true;
        try { await api("/api/hotels/" + button.dataset.hotelId + "/rooms/" + button.dataset.adminDeleteRoom, { method: "DELETE" }); await loadDashboard(); showDeleteMessage("Room deleted successfully.", false); } catch (error) { showDeleteMessage(error.message, true); } finally { button.disabled = false; }
      };
    });
    document.querySelectorAll("[data-admin-delete-manager]").forEach(function (button) {
      button.onclick = async function () {
        if (!confirm("Delete this manager account?")) return;
        try { await api("/api/managers/" + button.dataset.adminDeleteManager, { method: "DELETE" }); loadDashboard(); } catch (error) { alert(error.message); }
      };
    });
  }

  function installNavigation() {
    var area = document.querySelector("#account-area");
    if (!area || role() !== "Admin") return;
    var profileButton = area.querySelector("[data-page=\"profile\"]");
    if (profileButton) {
      profileButton.textContent = "Admin dashboard";
      profileButton.dataset.page = "admin";
      profileButton.onclick = function () { location.hash = "admin"; };
    }
    if (!area.querySelector("[data-page=\"admin\"]")) {
      var button = document.createElement("button");
      button.textContent = "Admin dashboard";
      button.dataset.page = "admin";
      button.onclick = function () { location.hash = "admin"; };
      area.insertBefore(button, area.firstChild);
    }
  }

  var area = document.querySelector("#account-area");
  if (area) new MutationObserver(installNavigation).observe(area, { childList: true });
  installNavigation();
  window.addEventListener("hashchange", function () {
    if (location.hash === "#admin") dashboard();
  });
  if (location.hash === "#admin") dashboard();
})();
