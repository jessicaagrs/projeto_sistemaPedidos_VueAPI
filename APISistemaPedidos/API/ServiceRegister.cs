﻿using API.Models.TipoDespesa;
using API.Infra.TipoDespesa;
using API.Services.TipoDespesa;
using API.Models.Fornecedor;
using API.Infra.Fornecedor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using API.Infra.Injector;
using API.Models;
using API.Services.Fornecedor;
using API.Services.Tributacao;
using API.Models.Tributacao;
using API.Infra.Tributacao;
using API.Models.Produto;
using API.Infra.Produto;
using API.Services.Produto;
using API.Models.Pedido;
using API.Infra.Pedido;

namespace Api
{
    public class ServiceRegister
    {
        public static void Register(IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSignalR();
            serviceDescriptors.AddSingleton<IUserIdProvider, NamedUserProvider>();
            serviceDescriptors.AddHttpContextAccessor();
            serviceDescriptors.AddSession();
            serviceDescriptors.AddDbContext<EFDBContext>();
            serviceDescriptors.AddTransient<ITipoDespesaRepositorio, TipoDespesaRepositorio>();
            serviceDescriptors.AddTransient<ITipoDespesaService, TipoDespesaService>();
            serviceDescriptors.AddTransient<IFornecedorRepositorio, FornecedorRepositorio>();
            serviceDescriptors.AddTransient<IFornecedorService, FornecedorService>();
            serviceDescriptors.AddTransient<ITributacaoRepositorio, TributacaoRepositorio>();
            serviceDescriptors.AddTransient<ITributacaoService, TributacaoService>();
            serviceDescriptors.AddTransient<IProdutoRepositorio, ProdutoRepositorio>();
            serviceDescriptors.AddTransient<IProdutoService, ProdutoService>();
            serviceDescriptors.AddTransient<IPedidoRepositorio, PedidoRepositorio>();
            serviceDescriptors.AddTransient<IPedidoService, PedidoService>();
        }
    }
}