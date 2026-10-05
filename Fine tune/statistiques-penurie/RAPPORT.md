# Statistiques de survie en pénurie

Expérience réalisée sur le moteur actuel, sans modifier les règles du jeu. Nourriture distribuée : 10 par jour ; agriculture, stocks, eau, relief, chef et diplomatie actifs dans le scénario principal. Graines 0 à 999, au maximum 200 jours par partie. Les scénarios de contrôle utilisent les mêmes graines.

**Dernier individu** : premier instant observé après un pas où un seul individu est encore vivant. Il peut ensuite mourir ; on mesure qui reste en dernier, pas son immortalité. **Dernière faction** : première faction seule encore vivante, même avec plusieurs habitants. Une extinction simultanée ne reçoit aucun dernier individu. Les parties qui atteignent 200 jours sans individu unique sont censurées ; elles peuvent avoir déjà une faction seule.

## Jeu actuel : 25 fondateurs

1000 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 126 | 12.6 % | 10.7–14.8 % | 145 |
| MAGA | 312 | 31.2 % | 28.4–34.1 % | 530 |
| Verts | 85 | 8.5 % | 6.9–10.4 % | 107 |
| Violets | 109 | 10.9 % | 9.1–13.0 % | 121 |

Extinctions sans dernier individu unique : 1. Sans dernier individu après 200 jours : 367. Sans dernière faction unique observée : 97.
Graine par défaut 42 : dernier individu de la faction Bleus au jour 41. Ce résultat se répète avec les mêmes règles lorsque l'on clique sur Recommencer.
Parmi les 320 derniers individus non-MAGA : Bleu 39.4 %, Vert 26.6 %, Violet 34.1 %. Test des trois fréquences égales : chi² = 7.956, 2 degrés de liberté, p = 0.018721.
Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.
Jour moyen du dernier individu : 75.1. Derniers individus à peau mixte : 69/632. La faction culturelle et la peau ne sont pas la même mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.007 | 0.993 | 1.005 |
| MAGA | 1.006 | 0.987 | 1.003 |
| Verts | 0.999 | 0.984 | 1.009 |
| Violets | 1.005 | 0.988 | 1.007 |

| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |
|---|---:|---:|
| Bleu | 115 | 155 |
| Corail | 282 | 326 |
| Vert | 75 | 98 |
| Violet | 91 | 122 |
Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s'additionne pas en nombre de parties.

## 24 fondateurs : 6 par faction

1000 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 94 | 9.4 % | 7.7–11.4 % | 108 |
| MAGA | 327 | 32.7 % | 29.9–35.7 % | 562 |
| Verts | 114 | 11.4 % | 9.6–13.5 % | 129 |
| Violets | 113 | 11.3 % | 9.5–13.4 % | 131 |

Extinctions sans dernier individu unique : 0. Sans dernier individu après 200 jours : 352. Sans dernière faction unique observée : 70.
Parmi les 321 derniers individus non-MAGA : Bleu 29.3 %, Vert 35.5 %, Violet 35.2 %. Test des trois fréquences égales : chi² = 2.374, 2 degrés de liberté, p = 0.30516.
Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.
Jour moyen du dernier individu : 76.0. Derniers individus à peau mixte : 90/648. La faction culturelle et la peau ne sont pas la même mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.003 | 0.987 | 0.988 |
| MAGA | 1.000 | 0.989 | 1.009 |
| Verts | 1.008 | 0.981 | 0.999 |
| Violets | 1.009 | 0.986 | 0.996 |

| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |
|---|---:|---:|
| Bleu | 76 | 123 |
| Corail | 304 | 349 |
| Vert | 90 | 137 |
| Violet | 88 | 129 |
Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s'additionne pas en nombre de parties.

## 24 fondateurs : factions non-MAGA permutées

1000 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 96 | 9.6 % | 7.9–11.6 % | 110 |
| MAGA | 334 | 33.4 % | 30.5–36.4 % | 567 |
| Verts | 91 | 9.1 % | 7.5–11.0 % | 105 |
| Violets | 121 | 12.1 % | 10.2–14.3 % | 135 |

Extinctions sans dernier individu unique : 0. Sans dernier individu après 200 jours : 358. Sans dernière faction unique observée : 83.
Parmi les 308 derniers individus non-MAGA : Bleu 31.2 %, Vert 29.5 %, Violet 39.3 %. Test des trois fréquences égales : chi² = 5.032, 2 degrés de liberté, p = 0.080763.
Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.
Jour moyen du dernier individu : 75.3. Derniers individus à peau mixte : 89/642. La faction culturelle et la peau ne sont pas la même mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.009 | 0.973 | 0.994 |
| MAGA | 1.001 | 0.987 | 1.009 |
| Verts | 0.988 | 0.980 | 0.993 |
| Violets | 1.003 | 0.982 | 1.002 |

| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |
|---|---:|---:|
| Bleu | 77 | 119 |
| Corail | 312 | 355 |
| Vert | 90 | 141 |
| Violet | 74 | 116 |
Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s'additionne pas en nombre de parties.

## 24 fondateurs : petit monde désactivé

1000 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 256 | 25.6 % | 23.0–28.4 % | 256 |
| MAGA | 235 | 23.5 % | 21.0–26.2 % | 235 |
| Verts | 259 | 25.9 % | 23.3–28.7 % | 259 |
| Violets | 250 | 25.0 % | 22.4–27.8 % | 250 |

Extinctions sans dernier individu unique : 0. Sans dernier individu après 200 jours : 0. Sans dernière faction unique observée : 0.
Parmi les 765 derniers individus non-MAGA : Bleu 33.5 %, Vert 33.9 %, Violet 32.7 %. Test des trois fréquences égales : chi² = 0.165, 2 degrés de liberté, p = 0.92095.
Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.
Jour moyen du dernier individu : 7.3. Derniers individus à peau mixte : 15/1000. La faction culturelle et la peau ne sont pas la même mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.016 | 0.990 | 1.004 |
| MAGA | 1.012 | 0.995 | 1.005 |
| Verts | 1.005 | 0.987 | 1.000 |
| Violets | 1.008 | 0.994 | 1.004 |

| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |
|---|---:|---:|
| Bleu | 250 | 258 |
| Corail | 231 | 238 |
| Vert | 256 | 265 |
| Violet | 248 | 254 |
Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s'additionne pas en nombre de parties.

## 24 fondateurs : raids et alliances désactivés

1000 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 221 | 22.1 % | 19.6–24.8 % | 288 |
| MAGA | 32 | 3.2 % | 2.3–4.5 % | 40 |
| Verts | 226 | 22.6 % | 20.1–25.3 % | 276 |
| Violets | 200 | 20.0 % | 17.6–22.6 % | 252 |

Extinctions sans dernier individu unique : 0. Sans dernier individu après 200 jours : 321. Sans dernière faction unique observée : 144.
Parmi les 647 derniers individus non-MAGA : Bleu 34.2 %, Vert 34.9 %, Violet 30.9 %. Test des trois fréquences égales : chi² = 1.765, 2 degrés de liberté, p = 0.41373.
Ce test porte sur les parties résolues par un individu unique. Les parties censurées limitent la portée de cette comparaison ; les scénarios sont des essais du même moteur, pas des observations biologiques.
Jour moyen du dernier individu : 106.1. Derniers individus à peau mixte : 76/679. La faction culturelle et la peau ne sont pas la même mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.005 | 0.989 | 0.996 |
| MAGA | 0.999 | 0.951 | 0.990 |
| Verts | 0.999 | 0.989 | 0.999 |
| Violets | 1.009 | 0.980 | 0.997 |

| Marqueur de peau | Derniers individus de peau unie | Derniers individus porteurs |
|---|---:|---:|
| Bleu | 196 | 239 |
| Corail | 26 | 67 |
| Vert | 203 | 236 |
| Violet | 178 | 213 |
Les porteurs de peau mixte sont comptés dans leurs deux couleurs : cette colonne ne s'additionne pas en nombre de parties.

## Interprétation des contrôles

Le jeu actuel contient un déséquilibre initial mesurable : sept Bleus contre six dans chacune des autres factions. Dans le contrôle à six par faction, les fréquences des trois factions non-MAGA sont compatibles avec des probabilités égales (voir le test ci-dessus). Cela suggère que l'effectif initial contribue à l'avantage bleu ; ce contrôle ne prouve pas que tous les autres effets sont nuls.
La permutation des positions des factions non-MAGA ne montre pas non plus d'avantage systématique bleu dans ce lot. La seule distance initiale aux MAGA ne suffit donc pas à expliquer les répétitions observées avec la graine 42. La carte reste asymétrique et de petits effets de position peuvent subsister.
Sans petit monde, les quatre factions donnent des nombres de derniers individus proches. Ce contrôle ne révèle pas de bonus intrinsèque lié à la couleur dans le modèle de base.
Désactiver les raids et les alliances change fortement les résultats MAGA, alors que leur chef, leur escorte, le relief et les ressources restent actifs. Le comportement de conflit joue donc un rôle majeur dans ces lots ; ce contrôle modifie deux mécanismes à la fois et ne sépare pas l'effet propre des raids de celui des alliances.
Les parties sans dernier individu unique à 200 jours ne sont pas des victoires attribuées au groupe le plus nombreux. L'agriculture et les réserves permettent des populations durables. Pour une comparaison plus longue, relancer avec une limite de jours plus grande et conserver la même définition des résultats.

## Lecture du code et limites

- La graine par défaut est 42. « Recommencer » reproduit la même expérience avec les mêmes règles : les relances ne sont pas indépendantes. Pour ce lot, chaque partie possède une graine différente.
- Avec 25 fondateurs distribués par i % 4, les Bleus commencent à 7, les autres à 6. Les Bleus disposent notamment d'un artisan supplémentaire. Avec 24, toutes les factions commencent à 6 avec la même répartition de métiers.
- La carte fixe ne présente pas de symétrie entre ouest, nord et sud : les lacs, montagnes et dépôts affectent les trajets, l'eau et les métiers. L'opposition initiale aux MAGA est une autre différence ; les côtés de départ sont retirés aléatoirement les jours suivants.
- Les traits suivent la même distribution initiale, mais les individus possèdent des génomes différents ; un petit échantillon ne garantit pas les mêmes phénotypes, sexes ou descendants par faction.
- Le commerce traite les paires de factions dans un ordre fixe, et la planification des raids parcourt les cibles par indice. Des effets de cet ordre sont possibles ; le scénario de permutation aide à distinguer les positions des noms de faction sans isoler chaque mécanisme.
- La permutation conserve les mêmes organismes, gènes, sexes, métiers, positions et carte, et change seulement les étiquettes culturelles des trois factions non-MAGA : ouest devient Vert, nord Violet, sud Bleu. Les MAGA restent à l'est. Les réserves initiales de ces trois camps sont identiques.
- Le contrôle neutre désactive tout le petit monde : pas de relief fonctionnel, eau, chef, réserves ni raids. Il vérifie la symétrie du modèle de base ; il ne représente pas un monde avec des MAGA particuliers.
- Le contrôle sans raids conserve le relief, les ressources, le chef et l'avidité de collecte, mais désactive les raids et les alliances.

Les CSV contiennent un enregistrement par graine, la faction du dernier individu, les deux marqueurs de peau, les traits, le métier, le sexe, le jour, les extinctions et les effectifs restants à la limite. Les XML donnent les règles exactes. `version-moteur.csv` conserve les empreintes SHA-256 des sources. Exécution : `powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Simuler-Penurie.ps1`.
