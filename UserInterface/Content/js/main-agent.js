jQuery(document).ready(function($){
	//create the slider
	$('.agent-testimonials-wrapper').flexslider({
		selector: ".cd-testimonials1 > li",
		animation: "slide",
		controlNav: false,
		slideshow: false,
		smoothHeight: true,
		start: function(){
			$('.agent-testimonials-wrapper').children('li').css({
				'opacity': 1,
				'position': 'relative'
			});
		}
	});

	//open the testimonials modal page
	$('.cd-see-all').on('click', function(){
		$('.agent-testimonials-all').addClass('is-visible');
	});

	//close the testimonials modal page
	$('.agent-testimonials-wrapper .close-btn').on('click', function(){
		$('.agent-testimonials-all').removeClass('is-visible');
	});
	$(document).keyup(function(event){
		//check if user has pressed 'Esc'
    	if(event.which=='27'){
    		$('.agent-testimonials-all').removeClass('is-visible');	
	    }
    });
    
	//build the grid for the testimonials modal page
	$('.agent-testimonials-all-wrapper').children('ul').masonry({
  		itemSelector: '.agent-testimonials-item'
	});
});