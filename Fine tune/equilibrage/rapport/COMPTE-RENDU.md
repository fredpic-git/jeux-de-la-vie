# Équilibrage des sept scénarios

250 graines par scénario et par lot, 40 jours simulés maximum. Aucun rendu graphique pendant les runs. Les réglages et les sources exactes sont archivés avec chaque lot.

| Scénario | Population finale avant → après | Runs avec raids avant → après | Avec coalitions avant → après | Châteaux endommagés avant → après | Armes produites après (moyenne) |
|---|---:|---:|---:|---:|---:|
| Sans mutations | 314.2 → 100.7 | 0.8 % → 6.4 % | 0.8 % → 2.4 % | 0.8 % → 6.4 % | 274.7 |
| Mutation vitesse | 318.8 → 102.9 | 2.4 % → 4.0 % | 2.4 % → 2.0 % | 2.4 % → 4.0 % | 278.0 |
| Trois traits | 315.9 → 101.5 | 1.6 % → 4.0 % | 1.6 % → 2.0 % | 1.6 % → 4.0 % | 274.4 |
| Raréfaction | 93.7 → 38.4 | 0.4 % → 34.8 % | 0.4 % → 19.2 % | 0.4 % → 34.8 % | 199.4 |
| Pénurie | 141.7 → 21.7 | 0.0 % → 75.6 % | 0.0 % → 35.2 % | 0.0 % → 75.6 % | 163.2 |
| Conflit MAGA | 265.7 → 27.8 | 100.0 % → 100.0 % | 100.0 % → 99.2 % | 100.0 % → 100.0 % | 224.9 |
| Grand monde | 627.2 → 182.0 | 58.8 % → 72.4 % | 58.8 % → 72.0 % | 58.8 % → 72.4 % | 686.1 |

## Lecture des résultats

Dans le scénario sans mutations, le moteur initial produisait déjà 704.7 armes par run et atteignait un pic moyen de 309.7 habitants équipés. Un stock d'armes faible dans le tableau n'indiquait donc pas une absence de fabrication : les armes sont distribuées à l'aube. Le panneau affiche maintenant séparément les habitants armés.

La population finale moyenne de ce scénario passe de 314.2 à 100.7 pour 200 fondateurs. La démographie est davantage contenue, sans garantie de stabilité pour toutes les graines. Les tableaux et courbes permettent de distinguer pénurie locale, ressources non collectées, stocks et capacité de production.

En pénurie, 75.6 % des validations présentent des raids, dont 35.2 % avec des attaques de coalition. Dans Conflit MAGA, ces proportions sont 100.0 % et 99.2 %. La confiance moyenne envers MAGA au dernier jour vaut -94.0, avec 0.0 alliés en moyenne. Ces indicateurs incluent les factions disparues : consulter les courbes et les données par graine pour interpréter leur évolution.

La référence Conflit MAGA présentait déjà des attaques dans 100.0 % des runs sur cet horizon, avec 90.9 assauts en moyenne et une confiance finale moyenne de -99.2. Les données ne confirment donc pas une absence générale de raids ou une amitié durable envers MAGA dans la version initiale ; la pénurie sans MAGA constituait le blocage principal observé.

Un raid organisé ne garantit pas un assaut exécuté : il faut atteindre le château avant le retour obligatoire, et une coalition doit réunir plusieurs factions sur place. Des parties peuvent donc rester pacifiques. Un château peut être endommagé puis réparé ; le minimum mesuré à chaque pas conserve la trace de l'assaut.

## Gagnants du lot de validation

Une victoire désigne une seule faction encore vivante au terme du run. Une coexistence à la limite des jours ne constitue pas une victoire de la faction la plus nombreuse.

| Scénario | Bleus | Corail / MAGA | Verts | Violets | Extinction totale | Coexistence à la limite |
|---|---:|---:|---:|---:|---:|---:|
| Sans mutations | 0 | 0 | 0 | 0 | 0 | 250 |
| Mutation vitesse | 0 | 0 | 0 | 0 | 0 | 250 |
| Trois traits | 0 | 0 | 0 | 0 | 0 | 250 |
| Raréfaction | 2 | 4 | 1 | 4 | 1 | 238 |
| Pénurie | 3 | 2 | 3 | 6 | 1 | 235 |
| Conflit MAGA | 0 | 4 | 0 | 0 | 0 | 246 |
| Grand monde | 0 | 0 | 0 | 0 | 0 | 250 |

## Choix retenus et limites

- Nourriture de survie : 1 ; eau personnelle : 1 ; reproduction : 3 nourritures par parent, au lieu de 2.
- Sacs : 3 unités pour les métiers ordinaires, 8 pour les transporteurs ; bonus MAGA de 2 unités. Priorité au transport d'eau si le château manque de réserve.
- Agriculture : 2 terres, 1 eau et 1 énergie donnent 3 nourritures.
- Énergie naturelle : 1 gisement de 2 unités de base par quart de carte, au lieu de 3 gisements de 3 ; quantités adaptées à la population. Le charbon retrouve un rôle dans la production.
- Réparation : 3 points de défense par nuit, avec 1 fer et 2 énergies disponibles ; maximum de 4 agrandissements de stockage.
- Diplomatie : la pénurie et les écarts de richesse dégradent les relations avec les camps qui concentrent les ressources ; les moins riches se rapprochent. Deux raids injustifiés rapprochés donnent une réputation d'agression qui bloque les alliances, puis celle-ci décroît de 1 point par nuit. Les contre-attaques contre un agresseur connu ne portent pas la même pénalité.
- Raids hostiles accessibles à toutes les factions, même sans coalition ; choix des cibles de coalition mélangé pour éviter un avantage systématique de couleur. Aucun raid contre une faction disparue.
- Grand monde conserve ses deux blocs permanents ; ces liens priment sur la diplomatie économique.
- Génétique conservée : 0,0025 par copie de gène, amplitude 0,2, brassage 0,12 et dominance 0,35. Les courbes montrent les traits moyens, pas uniquement les mutations : sélection et brassage changent aussi la population.

Ces choix répondent à des objectifs de jeu (démographie plus contenue, chaînes de production utiles, diplomatie réactive et conflits effectivement visibles). Ils ne prouvent ni un optimum global ni une représentation scientifique de sociétés humaines. Les pilotes utilisaient les graines 0 à 7 ; le lot final utilise 1000 à 1249, distinctes des graines exploratoires.

Les indicateurs de combat sont cumulés et les dégâts mesurés à chaque pas, avant réparation. Les quantités sur la carte et celles dans les châteaux sont séparées ; les stocks des châteaux dépeuplés restent comptés car ils existent encore. Les colonnes consommation_ mesurent uniquement les transformations, constructions et réparations ; les rations, échanges, pillages et pertes ne sont pas inclus dans ces compteurs. La production compte les ressources fabriquées, y compris les excédents perdus si les réserves sont pleines. Chaque journée est moyennée sur tous les runs, y compris les extinctions : aucune suppression des runs défavorables. Après extinction totale, la population et la production restent à zéro et les dernières quantités de stocks et de gisements sont conservées pour les bilans restants ; le moteur ne poursuit pas les renouvellements sans habitants. Les traits sont pondérés par les habitants vivants ; la série à zéro population ne donne pas de trait moyen.

Avec 250 runs, la précision d'une proportion proche de 50 % est d'environ ±6 points à 95 %. Ne pas interpréter un écart de quelques points entre couleurs comme une preuve de biais. Les gisements sont renouvelés à chaque aube ; la raréfaction programmée concerne la nourriture naturelle. Les stocks alimentaires servent aux rations de survie, tandis que les parents doivent atteindre le seuil de nourriture sur le terrain pour former un couple ; un stock élevé ne suffit donc pas à garantir des naissances. La durée de 40 jours limite l'observation des mutations rares et ne permet pas d'affirmer qui gagnerait toutes les coexistences prolongées.

Ouvrir [le rapport graphique](Rapport-Equilibrage.html) pour consulter les courbes et [comparaison.csv](comparaison.csv) pour les chiffres synthétiques. Les CSV quotidiens des lots donnent tous les stocks, consommations, productions et quantités par graine et faction.
