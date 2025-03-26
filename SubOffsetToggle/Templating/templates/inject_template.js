function injectCSS() {
    if (document.getElementById("subOffsetStyle")) {
        return; // Prevent duplicate injection
    }

    const css = `
    .subOffTglNotif {
        position: absolute;
        top: 10em;
        width: 100%;
        display: flex;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease-in-out;
    }

    .subOffTglNotif-content {
        position: relative;
        height: 3em;
        border-radius: var(--largeRadius);
        background-color: rgba(69, 69, 69, 0.69);
        backdrop-filter: var(--blurDefault);
        color: #fff;
        width: 4em;
        text-align: center;
        line-height: 3em;
        font-size: 20px;
    }
    
    .show_toggle {
        opacity: 1;
    }`;

    const style = document.createElement("style");
    style.id = "subOffsetStyle"; // Unique ID to prevent duplicate injection
    style.innerHTML = css;
    document.head.appendChild(style);
}

let lastClickTime = 0;
let hideTimeout;

function add_new_btn_next_to_sub(){
    injectCSS();
    if (document.getElementById("btnToggleOffset")){
        return;
    }

    const subtitles_btn = document.getElementsByClassName("btnSubtitles")[0];

    const offsetBtnHTML = `<button id="btnToggleOffset" is="paper-icon-button-light" class="btnSubtitles autoSize paper-icon-button-light" title="Toggle Sub Offset"> 
                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M12 2C8.13 2 5 5.13 5 9C5 12.87 8.13 16 12 16C15.87 16 19 12.87 19 9C19 5.13 15.87 2 12 2ZM12 14C9.24 14 7 11.76 7 9C7 6.24 9.24 4 12 4C14.76 4 17 6.24 17 9C17 11.76 14.76 14 12 14Z" fill="currentColor"/>
                                    <path d="M12 6V9L14 11" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                                    <path d="M8 18H16" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </button>`;
    subtitles_btn.insertAdjacentHTML('afterend', offsetBtnHTML);

    const toggle_btn = document.getElementById("btnToggleOffset");
    const sub_sync_head = document.getElementsByClassName("subtitleSync")[0];

    // Check if notification already exists, if not, create it
    if (!document.querySelector('.subOffTglNotif')) {
        const notifHTML = `
            <div class="subOffTglNotif">
                <div class="subOffTglNotif-content">0.0s</div>
            </div>`;
        sub_sync_head.insertAdjacentHTML('afterend', notifHTML);
    }

    toggle_btn.addEventListener("click", function() {
        const val1 = Number({{ offset_value_1 }});
        const val2 = Number({{ offset_value_2 }});
        
        let uniqueVal;
        let useUnique = false;

        if (val1 === 0 || val2 === 0) {
            useUnique = true;
            singleVal = val1 === 0 ? val2 : val1;
        }
        else if (val1 === val2){
            useUnique = true;
            singleVal = val1;
        }

        const offset_slider = sub_sync_head.getElementsByClassName("subtitleSyncSlider")[0];

        const storedVal = offset_slider.value*1;
        let newVal;

        // UseUnique is called if either val1 or val2 is equal to 0
        // Behavior will be toggling between singleVal and 0
        if (useUnique) {
            newVal = storedVal === 0 ? uniqueVal : 0;
        }
        else { 
            // Cycle val1 -> val2 -> 0 -> val1
            // Default to 0 if storedVal is not val1 or val2
            if (storedVal === val1) {
                newVal = val2;
            }
            else if (storedVal === val2 || storedVal !== 0) {
                newVal = 0;
            }
            else {
                newVal = val1;
            }
        }
        
        offset_slider.value = "" + newVal
        const changeEvent = new Event('change');
        offset_slider.dispatchEvent(changeEvent);

        // Display change of offset
        if (sub_sync_head.querySelector(".subtitleSyncContainer").classList.contains('hide')){
            showSubOffsetNotification(newVal);
        }

        // Reset timeout if a new click happens before the previous one hides
        clearTimeout(hideTimeout);
        lastClickTime = Date.now();

        hideTimeout = setTimeout(() => {
        // Only hide after the last click, if this was the most recent click
            let notif = document.querySelector('.subOffTglNotif');
            if (notif) {
                notif.classList.remove('show_toggle');
            }
        }, 2000);
    });
}

// Function to show and fade out the notification
function showSubOffsetNotification(offset) {
    let notif = document.querySelector('.subOffTglNotif');

    if (notif) {
        notif.querySelector('.subOffTglNotif-content').textContent = `${offset}s`;
        notif.classList.add('show_toggle');
    }
}

let lastUrl = window.location.href;

setInterval(() => {
    if (window.location.href !== lastUrl) {
        lastUrl = window.location.href;
        checkUrlAndTrigger();
    }
}, 500); // Check every 500ms

function checkUrlAndTrigger() {
    if (window.location.href.includes("#/video")) {
        if (document.readyState === "loading") {
            // DOM is still loading, wait for DOMContentLoaded
            document.addEventListener("DOMContentLoaded", add_new_btn_next_to_sub);
        } else {
            // DOM is already loaded, run code immediately
            add_new_btn_next_to_sub();
        }
    }
}