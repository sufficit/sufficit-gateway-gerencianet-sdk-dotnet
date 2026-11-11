# Compatibilidade das dependências de consumidores

Objetivo: remover NU1608 em consumidores net7/net10 sem baixar as dependências do aplicativo.

1. [concluído] Ajustar limites superiores das três dependências Microsoft para aceitar versões até 10.x mantendo os pisos e as versões usadas na compilação.
2. [concluído] Compilar/empacotar os três frameworks e validar o restore do consumidor com o novo pacote.
3. [em andamento] Registrar atividade e fazer commit/push na main; conferir publicação automática existente.

Não modificar comportamento HTTP nem chamar endpoints reais. O grupo net7 mantém logging 7.x e JSON 8 no build, mas aceita versões posteriores do consumidor; os grupos netstandard/net9 continuam compilando com 9.x.

Build/pack Release aprovados nos três frameworks. Consumidor Standard compilou netstandard2.0/net7.0/net10.0 com o pacote em feed temporário; NU1608 desapareceu. Os conflitos de Logging exigiram alinhar a família Extensions 8 no consumidor net7. Testes históricos deste SDK usam APIs retiradas e chamadas externas; não foram executados.
