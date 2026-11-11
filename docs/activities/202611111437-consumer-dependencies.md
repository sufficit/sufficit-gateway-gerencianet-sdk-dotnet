# Intervalos de dependência dos consumidores

Os limites superiores <8/<9/<10 dos pacotes Microsoft geravam NU1608 nos consumidores que alinham logging e JSON em versões mais recentes. Mantidos os pisos e a família compilada por TFM, com aceitação de versões até 10.x (<11).

Build/pack Release de netstandard2.0/net7.0/net9.0 passou; restore/build do Standard nos três frameworks passou usando pacote local. Testes antigos net45 não executados: usam APIs já removidas e endpoints reais. Sem chamadas externas de negócio.

Push na main aciona o workflow Publish Packages existente. A publicação será conferida antes de atualizar o piso definitivo do Standard. [Plano](../PLAN-CONSUMER-DEPENDENCIES.md).
