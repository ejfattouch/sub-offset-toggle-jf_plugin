function wait(ms){
    return new Promise(resolve => setTimeout(resolve, ms));
}

function add_new_btn_next_to_sub(){
    if (document.getElementById("btnToggleOffset")){
        return;
    }

    const subtitles_btn = document.getElementsByClassName("btnSubtitles")[0];

    console.log(subtitles_btn)


    const offsetBtnHTML = `<button id="btnToggleOffset" is="paper-icon-button-light" class="btnSubtitles autoSize paper-icon-button-light" title="Toggle Sub Offset"> 
                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M12 2C8.13 2 5 5.13 5 9C5 12.87 8.13 16 12 16C15.87 16 19 12.87 19 9C19 5.13 15.87 2 12 2ZM12 14C9.24 14 7 11.76 7 9C7 6.24 9.24 4 12 4C14.76 4 17 6.24 17 9C17 11.76 14.76 14 12 14Z" fill="currentColor"/>
                                    <path d="M12 6V9L14 11" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                                    <path d="M8 18H16" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </button>`;
    subtitles_btn.insertAdjacentHTML('afterend', offsetBtnHTML);

    const toggle_btn = document.getElementById("btnToggleOffset");

    console.log(toggle_btn)

    toggle_btn.addEventListener("click", function() {
        const val1 = 0;
        const val2 = 5.8;
        
        const offset_slider = document.getElementsByClassName("subtitleSyncSlider")[0];

        const storedVal = offset_slider.value*1;
        let newVal;

        if (storedVal !== val1 && storedVal !== val2){
            newVal = val1
        }
        else{
            newVal = storedVal == val1 ? val2 : val1;
        }
        
        offset_slider.value = "" + newVal

        const changeEvent = new Event('change');
        
        offset_slider.dispatchEvent(changeEvent);
    });
}

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

let lastUrl = window.location.href;

setInterval(() => {
    if (window.location.href !== lastUrl) {
        lastUrl = window.location.href;
        checkUrlAndTrigger();
    }
}, 500); // Check every 500ms
