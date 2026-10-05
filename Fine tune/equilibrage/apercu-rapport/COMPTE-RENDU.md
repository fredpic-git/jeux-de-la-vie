# Équilibrage des sept scénarios

8 graines par scénario et par lot, 40 jours simulés maximum. Aucun rendu graphique pendant les runs. Les réglages et les sources exactes sont archivés avec chaque lot.

| Scénario | Population finale avant → après | Runs avec raids avant → après | Avec coalitions avant → après | Châteaux endommagés avant → après | Armes produites après (moyenne) |
|---|---:|---:|---:|---:|---:|
| Sans mutations | 92.5 → 92.5 | 0.0 % → 0.0 % | 0.0 % → 0.0 % | 0.0 % → 0.0 % | 279.4 |
| Mutation vitesse | 113.4 → 113.4 | 0.0 % → 0.0 % | 0.0 % → 0.0 % | 0.0 % → 0.0 % | 289.8 |
| Trois traits | 99.0 → 99.0 | 12.5 % → 12.5 % | 0.0 % → 0.0 % | 12.5 % → 12.5 % | 267.8 |
| Raréfaction | 32.4 → 32.4 | 25.0 % → 25.0 % | 25.0 % → 25.0 % | 25.0 % → 25.0 % | 186.9 |
| Pénurie | 21.8 → 21.8 | 87.5 % → 87.5 % | 50.0 % → 50.0 % | 87.5 % → 87.5 % | 165.3 |
| Conflit MAGA | 20.0 → 20.0 | 100.0 % → 100.0 % | 100.0 % → 100.0 % | 100.0 % → 100.0 % | 213.3 |
| Grand monde | 197.4 → 197.4 | 50.0 % → 50.0 % | 50.0 % → 50.0 % | 50.0 % → 50.0 % | 703.5 |

## Gagnants du lot de validation

Une victoire désigne une seule faction encore vivante au terme du run. Une coexistence à la limite des jours ne constitue pas une victoire de la faction la plus nombreuse.

| Scénario | Bleus | Corail / MAGA | Verts | Violets | Extinction totale | Coexistence à la limite |
|---|---:|---:|---:|---:|---:|---:|
| Sans mutations | 0 | 0 | 0 | 0 | 0 | 8 |
| Mutation vitesse | 0 | 0 | 0 | 0 | 0 | 8 |
| Trois traits | 0 | 0 | 0 | 0 | 0 | 8 |
| Raréfaction | 0 | 0 | 0 | 0 | 0 | 8 |
| Pénurie | 0 | 0 | 0 | 0 | 0 | 8 |
| Conflit MAGA | 0 | 0 | 0 | 0 | 0 | 8 |
| Grand monde | 0 | 0 | 0 | 0 | 0 | 8 |

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

Les indicateurs de combat sont cumulés et les dégâts mesurés à chaque pas, avant réparation. Les quantités sur la carte et celles dans les châteaux sont séparées ; les stocks des châteaux dépeuplés restent comptés car ils existent encore. Chaque journée est moyennée sur tous les runs, y compris les extinctions : aucune suppression des runs défavorables. Les traits sont pondérés par les habitants vivants ; la série à zéro population ne donne pas de trait moyen.

Avec 250 runs, la précision d'une proportion proche de 50 % est d'environ ±6 points à 95 %. Ne pas interpréter un écart de quelques points entre couleurs comme une preuve de biais. La durée de 40 jours limite l'observation des mutations rares et ne permet pas d'affirmer qui gagnerait toutes les coexistences prolongées.

Ouvrir [le rapport graphique](Rapport-Equilibrage.html) pour consulter les courbes et [comparaison.csv](comparaison.csv) pour les chiffres synthétiques. Les CSV quotidiens des lots donnent tous les stocks, consommations, productions et quantités par graine et faction.
