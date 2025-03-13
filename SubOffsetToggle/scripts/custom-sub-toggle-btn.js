function wait(ms){
    return new Promise(resolve => setTimeout(resolve, ms));
}

function load_btn_play(e) {
    const btnPlay_observer = new MutationObserver((mutationsList, observer) => {
        const btnPlay = document.getElementsByClassName("btnPlay")[0];

        if (btnPlay) {
            observer.disconnect();
            add_el_to_btn(btnPlay);
        }
    });

    btnPlay_observer.observe(document.body, {childList: true, subtree: true});
}


async function add_el_to_btn(btnPlay) {
    btnPlay.addEventListener('click', async function() {
        const spinner = document.getElementsByClassName("mdl-spinner")[0];

        const spinnerObserver = new MutationObserver(async (mutationsList, observer) => {
            for (let mutation of mutationsList) {
                if (mutation.attributeName === "class") {
                    if (!spinner.classList.contains("spinnerMdlActive")) {                        
    
                        observer.disconnect(); // Stop observing after the class is removed
                        
                        await wait(2000); // Pause for 2 seconds

                        add_new_btn_next_to_sub();
                    }
                }
            }
        });

        spinnerObserver.observe(spinner, { attributes: true, attributeFilter: ["class"] });
    });
}

function add_new_btn_next_to_sub(){
    if (document.getElementById("btnToggleOffset")){
        return;
    }

    const subtitles_btn = document.getElementsByClassName("btnSubtitles")[0];

    console.log(subtitles_btn)


    const offsetBtnHTML = `<button id="btnToggleOffset" is="paper-icon-button-light" class="btnSubtitles autoSize paper-icon-button-light" title="Subtitles"> 
                                <span class="xlargePaperIconButton material-icons closed_caption" aria-hidden="true"></span> 
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
    console.log("GAGA")
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
