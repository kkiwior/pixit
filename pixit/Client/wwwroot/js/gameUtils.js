window.handleResize = () => {
    window.addEventListener('resize', resize, false);
    resize();    
}

function resize(){
    let players = parseInt(document.querySelector('.card-container').dataset.players);
    let height = window.innerHeight;
    let width = document.querySelector('.card-container').getBoundingClientRect().width;
    let c = 200, h = 300;
    
    for(var i = 1; i <= 20; ++i)
    {
        let cols = Math.ceil(players / i);
        
        cf = Math.floor(width / cols);
        hf = c * 1.5;
        
        console.log(cf, hf, width, height);

        if(cf * 1.5 * i >= height) {
            h = Math.floor(height / i);
            c = Math.floor(h * 0.667);
            console.log(h, c);
            console.log("break");
            break;
        }
    }
    
    
    document.querySelectorAll('.cardc').forEach(card =>{
        card.style.width = c + 'px';
        card.style.height = 1.5 * c + 'px';
    })
}