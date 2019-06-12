using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using LightPlugin.Core;
using LightPlugin.Core.Caching;
using LightPlugin.Core.Configuration;
using LightPlugin.Core.Data;
using LightPlugin.Core.Fakes;
using LightPlugin.Core.Infrastructure;
using LightPlugin.Core.Infrastructure.DependencyManagement;
using LightPlugin.Core.Plugins;
//using LightPlugin.Data;
//using LightPlugin.Services.Affiliates;
//using LightPlugin.Services.Authentication;
//using LightPlugin.Services.Authentication.External;
//using LightPlugin.Services.Blogs;
//using LightPlugin.Services.Catalog;
//using LightPlugin.Services.Cms;
//using LightPlugin.Services.Common;
//using LightPlugin.Services.Configuration;
//using LightPlugin.Services.Customers;
//using LightPlugin.Services.Directory;
//using LightPlugin.Services.Discounts;
//using LightPlugin.Services.Events;
//using LightPlugin.Services.ExportImport;
//using LightPlugin.Services.Forums;
//using LightPlugin.Services.Helpers;
//using LightPlugin.Services.Installation;
//using LightPlugin.Services.Localization;
//using LightPlugin.Services.Logging;
//using LightPlugin.Services.Media;
//using LightPlugin.Services.Messages;
//using LightPlugin.Services.News;
//using LightPlugin.Services.Orders;
//using LightPlugin.Services.Payments;
//using LightPlugin.Services.Polls;
//using LightPlugin.Services.Security;
//using LightPlugin.Services.Seo;
//using LightPlugin.Services.Shipping;
//using LightPlugin.Services.Stores;
//using LightPlugin.Services.Tasks;
//using LightPlugin.Services.Tax;
//using LightPlugin.Services.Topics;
//using LightPlugin.Services.Vendors;
//using LightPlugin.Web.Framework.Mvc.Routes;
//using LightPlugin.Web.Framework.Themes;
//using LightPlugin.Web.Framework.UI;

namespace LightPlugin.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c => 
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            
            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
           


            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();

           
            
            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_per_request").InstancePerLifetimeScope();


            //work context
            //store context

            //services
            //builder.RegisterType<BackInStockSubscriptionService>().As<IBackInStockSubscriptionService>().InstancePerLifetimeScope();
            //builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
            //builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerLifetimeScope();
            //builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerLifetimeScope();
            //builder.RegisterType<ManufacturerService>().As<IManufacturerService>().InstancePerLifetimeScope();
            //builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            //builder.RegisterType<CopyProductService>().As<ICopyProductService>().InstancePerLifetimeScope();
            //builder.RegisterType<SpecificationAttributeService>().As<ISpecificationAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductTemplateService>().As<IProductTemplateService>().InstancePerLifetimeScope();
            //builder.RegisterType<CategoryTemplateService>().As<ICategoryTemplateService>().InstancePerLifetimeScope();
            //builder.RegisterType<ManufacturerTemplateService>().As<IManufacturerTemplateService>().InstancePerLifetimeScope();
            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<ProductTagService>().As<IProductTagService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<AddressAttributeFormatter>().As<IAddressAttributeFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<AddressAttributeParser>().As<IAddressAttributeParser>().InstancePerLifetimeScope();
            //builder.RegisterType<AddressAttributeService>().As<IAddressAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            //builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerLifetimeScope();
            //builder.RegisterType<VendorService>().As<IVendorService>().InstancePerLifetimeScope();
            //builder.RegisterType<SearchTermService>().As<ISearchTermService>().InstancePerLifetimeScope();
            //builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerLifetimeScope();
            //builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerLifetimeScope();


            //builder.RegisterType<CustomerAttributeParser>().As<ICustomerAttributeParser>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomerAttributeService>().As<ICustomerAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomerReportService>().As<ICustomerReportService>().InstancePerLifetimeScope();

            //pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<PermissionService>().As<IPermissionService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();
            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<AclService>().As<IAclService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();
            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<GeoLookupService>().As<IGeoLookupService>().InstancePerLifetimeScope();
            //builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            //builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerLifetimeScope();
            //builder.RegisterType<MeasureService>().As<IMeasureService>().InstancePerLifetimeScope();
            //builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerLifetimeScope();

            //builder.RegisterType<StoreService>().As<IStoreService>().InstancePerLifetimeScope();
            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<StoreMappingService>().As<IStoreMappingService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<DiscountService>().As<IDiscountService>().InstancePerLifetimeScope();


            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<SettingService>().As<ISettingService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();
            //builder.RegisterSource(new SettingsSource());

            ////pass MemoryCacheManager as cacheManager (cache locales between requests)
            //builder.RegisterType<LocalizationService>().As<ILocalizationService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            ////pass MemoryCacheManager as cacheManager (cache locales between requests)
            //builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerLifetimeScope();

            //builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerLifetimeScope();
            //builder.RegisterType<PictureService>().As<IPictureService>().InstancePerLifetimeScope();

            //builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerLifetimeScope();
            //builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerLifetimeScope();
            //builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerLifetimeScope();
            //builder.RegisterType<CampaignService>().As<ICampaignService>().InstancePerLifetimeScope();
            //builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerLifetimeScope();
            //builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerLifetimeScope();
            //builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerLifetimeScope();
            //builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();

            //builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>().InstancePerLifetimeScope();
            //builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerLifetimeScope();
            //builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            //builder.RegisterType<OrderReportService>().As<IOrderReportService>().InstancePerLifetimeScope();
            //builder.RegisterType<OrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();
            //builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerLifetimeScope();
            //builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();

            //builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();

            //builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();
            //builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();


            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<UrlRecordService>().As<IUrlRecordService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<ShipmentService>().As<IShipmentService>().InstancePerLifetimeScope();
            //builder.RegisterType<ShippingService>().As<IShippingService>().InstancePerLifetimeScope();

            //builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerLifetimeScope();
            //builder.RegisterType<TaxService>().As<ITaxService>().InstancePerLifetimeScope();
            //builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().InstancePerLifetimeScope();

            //builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();

            ////pass MemoryCacheManager as cacheManager (cache settings between requests)
            //builder.RegisterType<CustomerActivityService>().As<ICustomerActivityService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
            //    .InstancePerLifetimeScope();

            //if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseFastInstallationService"]) &&
            //    Convert.ToBoolean(ConfigurationManager.AppSettings["UseFastInstallationService"]))
            //{
            //    builder.RegisterType<SqlFileInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
            //}
            //else
            //{
            //    builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
            //}

            //builder.RegisterType<ForumService>().As<IForumService>().InstancePerLifetimeScope();

            //builder.RegisterType<PollService>().As<IPollService>().InstancePerLifetimeScope();
            //builder.RegisterType<BlogService>().As<IBlogService>().InstancePerLifetimeScope();
            //builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerLifetimeScope();
            //builder.RegisterType<TopicService>().As<ITopicService>().InstancePerLifetimeScope();
            //builder.RegisterType<NewsService>().As<INewsService>().InstancePerLifetimeScope();

            //builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            //builder.RegisterType<SitemapGenerator>().As<ISitemapGenerator>().InstancePerLifetimeScope();
            //builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();

            //builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();

            //builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();
            //builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerLifetimeScope();
            //builder.RegisterType<PdfService>().As<IPdfService>().InstancePerLifetimeScope();
            //builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerLifetimeScope();


            //builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerLifetimeScope();
            //builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerLifetimeScope();
           
                
            //builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            ////Register event consumers
            //var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            //foreach (var consumer in consumers)
            //{
            //    builder.RegisterType(consumer)
            //        .As(consumer.FindInterfaces((type, criteria) =>
            //        {
            //            var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
            //            return isMatch;
            //        }, typeof(IConsumer<>)))
            //        .InstancePerLifetimeScope();
            //}
            //builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            //builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();

        }

        public int Order
        {
            get { return 0; }
        }
    }


    //public class SettingsSource : IRegistrationSource
    //{
    //    static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
    //        "BuildRegistration",
    //        BindingFlags.Static | BindingFlags.NonPublic);

    //    public IEnumerable<IComponentRegistration> RegistrationsFor(
    //            Service service,
    //            Func<Service, IEnumerable<IComponentRegistration>> registrations)
    //    {
    //        var ts = service as TypedService;
    //        if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
    //        {
    //            var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
    //            yield return (IComponentRegistration)buildMethod.Invoke(null, null);
    //        }
    //    }

    //    //    static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
    //    //    {
    //    //        return RegistrationBuilder
    //    //            .ForDelegate((c, p) =>
    //    //            {
    //    //                var currentStoreId = c.Resolve<IStoreContext>().CurrentStore.Id;
    //    //                //uncomment the code below if you want load settings per store only when you have two stores installed.
    //    //                //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
    //    //                //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

    //    //                //although it's better to connect to your database and execute the following SQL:
    //    //                //DELETE FROM [Setting] WHERE [StoreId] > 0
    //    //                return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentStoreId);
    //    //            })
    //    //            .InstancePerLifetimeScope()
    //    //            .CreateRegistration();
    //    //    }

    //    //    public bool IsAdapterForIndividualComponents { get { return false; } }
    //}

}
