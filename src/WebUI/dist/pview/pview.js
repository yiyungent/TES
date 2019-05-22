function Pview(options) {

	this.init = function () {
		var that = this;
		$('[pview-btn]').on('click', function (e) {
			// 阻止默认事件
			e.preventDefault();
			console.log('init start');
			var goUrl = $(this).attr('href');
			var pview = $(this).attr('pview-targets');
			console.log(pview);

			that.go(pview, goUrl, 'get', {});
		});
	},

	/**
	 * 前往此页面
	 * @param {any} pview 指定要更新的区块 eg: top-nav,main-content
	 * @param {any} url	eg: www.baidu.com
	 * @param {any} type eg: get or post
	 * @param {any} data 要发送的数据
	 */
	this.go = function (pview, url, type, data) {
		$.ajax({
			url: url,
			type: type,
			headers: { "pview": pview },
			data: data,
			dataType: 'html',
			success: function (data) {
				// 包装数据
				var $dataObj = $('<code></code>').append($(data));

				// 需要更新的区块
				var updatePviews = [];
				var updatePviewNames = pview.split(',');
				for (var i = 0; i < updatePviewNames.length; i++) {
					updatePviews[i] = $('[pview="' + updatePviewNames[i] + '"]')[0]; // 注意：每个是一个 js对象
				}
				// 查找当前页面的每一个 pview块, 并将返回的 html 从中筛选出 pview块，将对应的 旧的 pview块替换
				console.log('---------updatePviews-----------');
				console.log(updatePviews);
				var pviewItemName = '', pviewItemHtml = '';
				for (var i = 0; i < updatePviews.length; i++) {
					// 当前 pview块的 名
					pviewItemName = $(updatePviews[i]).attr('pview');
					// 当前 pview块的 新返回的 html
					pviewItemHtml = $('[pview="' + pviewItemName + '"]', $dataObj).html();
					console.log('---------updatePviewItem-----------');
					console.log(updatePviews[i]);
					updatePviews[i].innerHTML = pviewItemHtml;
					console.log('------success-----');
				}
			}
		});
	}


}
