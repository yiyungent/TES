function PView(options) {

	this.init = function () {
		var that = this;
		$('[pview-btn]').on('click', function (e) {
			console.log('init start');
			// 阻止默认事件
			e.preventDefault();
			var goUrl = '';
			if (this.hasAttribute('pview-url')) {
				goUrl = this.getAttribute('pview-url');
			} else if (this.tagName.toLowerCase() == 'a') {
				goUrl = this.getAttribute('href');
			} else {
				goUrl = window.location.href;
			}

			var pview = ''
			if (this.hasAttribute('pview-targets')) {
				pview = this.getAttribute('pview-targets');
			}

			that.go(pview, goUrl, 'get', {});
			console.log('init success');
		});
	},

	/**
	 * 前往此页面
	 * @param {String} pview 指定要更新的区块 eg: top-nav,main-content
	 * @param {String} url	eg: www.baidu.com
	 * @param {String} type eg: get or post
	 * @param {String | Object} data 要发送的数据
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
				console.log($dataObj);

				// 需要更新的区块
				var updatePviews = [];
				var updatePviewNames = [];
				if (pview != null && pview.trim() != '') {
					updatePviewNames = pview.split(',');
				}
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
