# SecretaryProblem

## 문제 소개 - Secretary Problem
<div align="center">
  <table>
    <tr>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/c0b121ba-3f7d-4522-b388-8251d1f5f9e3" width="960px" height="540px"/></th>
    </tr>
  </table>
</div>

<Secretary Problem (비서 문제)> 는 비서를 고용하는 과정으로 문제를 설명할 수 있습니다.

최고의 비서를 고용하고자 하는 면접관을 상상해보겠습니다.  
n명의 지원자가 있고, n명의 지원자는 1등부터 n등까지 순위를 매길 수 있습니다.  
면접관은 지원자 중 한 명씩 무작위로 인터뷰를 진행합니다.  

지원자의 고용 여부는 해당 지원자의 인터뷰 직후에 이뤄집니다.  
지원자를 탈락시키면, 결정을 번복할 수 없습니다.  

면접관은 지금까지 인터뷰한 모든 지원자들에 대해서 순위를 매길 수 있습니다.  
아직 인터뷰하지 않은 지원자의 순위는 매길 수 없습니다.  

이때, **<최고의 지원자를 선택하기 위해서는 어떻게 해야하는가?>** 에 대한 전략이  
Secretary Problem 입니다.  

## 접근법과 이에 대한 최적해
이 문제에 대한 접근 방법은 어떤 수 k를 정하여 k번째 지원자까지는 `무조건` 거절하는 것 입니다.  
k번째 까지 중 가장 순위가 좋았던 지원자를 기억하고, 그 지원자보다 뛰어난 지원자가 나타나면 바로 고용하는 방법을 통해 접근할 수 있습니다.  

그리고 이 경우 가장 최적의 해가 되는 k값은 $k = \frac {n-1} {e}$에 가장 가까운 자연수입니다.  

## 최적해 증명 과정
최고의 지원자의 인터뷰 순서를 $A$, 최고의 지원자를 고르게 되는 사건을 $B$라고 하겠습니다.  

최고 지원자의 순서가 $i$일 때, 면접관이 최고의 지원자를 뽑을 확률 $P_k(B|A=i)$를 구하겠습니다.  

$k \geq i$ 이면 $i$번째 지원자는 무조건 탈락 대상입니다.  
$k < i$ 이면 $[ 1, (i-1) ]$ 번째 지원자 중 최고 지원자의 순서가 $[ 1, k ]$ 범위에 존재하는 확률과 같습니다.  

따라서  

$$
P_k(B|A=i)=
\begin{cases}
0, & \mbox{if } k \geq i \\
\frac{k}{i-1}, & \mbox{if } k < i
\end{cases}
$$

<br>  

$k$값에 따른 최고의 지원자 고용 확률 $P_k(B)$ 를 구해보겠습니다.  
$A=i$  $(1 \leq i \leq n)$는 동시에 일어날 수 없는 상호 배타입니다.  

따라서  
  
$$
\begin{matrix}
P_k(B) &=& P_k(B\cap(A=1)) + \cdots + P_k(B\cap(A=n)) \\
       &=& P_k(B|A=1)P_k(A=1)+ \cdots + P_k(B|A=n)P_k(A=n) \\
       &=& \displaystyle\sum_{k=1}^{n} P_k(B|A=i)P_k(A=i)
\end{matrix}
$$

<br>  

최고의 지원자의 인터뷰 순서가 i (1<=i<=n) 일 각각의 확률은 $\frac{1}{n}$ 입니다.

따라서

$$
\begin{matrix}
P_k(B) &=& \displaystyle\sum_{i=1}^{n} P_k(B|A=i)P_k(A=i) \\
       &=& \displaystyle\sum_{i=1}^{k} 0 * \frac{1}{n} + \displaystyle\sum_{i=k+1}^{n} \frac{k}{i-1} * \frac{1}{n} \\
       &=& \frac{k}{n}\displaystyle\sum_{i=k+1}^{n} \frac{1}{i-1}
\end{matrix}
$$

<br>  

$n$이 충분히 클 때 $P_k(B) = \frac{k}{n}\displaystyle\sum_{i=k+1}^{n} \frac{1}{i-1}$ 는 $\frac{k}{n} \int_{k}^{n-1} \frac{1}{x}\, dx$에 근사한다고 볼 수 있습니다.

따라서

$$
\begin{matrix}
P_k(B) &=& \frac{k}{n} \int_{k}^{n-1} \frac{1}{x}\, dx \\
       &=& \frac{k}{n}(ln(n-1)-lnk) \\
\\
\frac{d}{dk}P_k(B) &=& \frac{1}{n}(\ln(n-1)-lnk)-\frac{1}{n} \\
                   &=& \frac{1}{n}(\ln\frac{n-1}{k}-1)
\end{matrix}
$$

<br>

$k < \frac{n-1}{e}$ 일 때 $\frac{d}{dk}P_k(B) > 0$ 이고,  
$k > \frac{n-1}{e}$ 일 때 $\frac{d}{dk}P_k{B} < 0$ 이므로  
$P_k(B)$는 $k$가 $\frac{n-1}{e}$ 에 가장 가까운 자연수일 때 최댓값을 가집니다.

그리고 이를 이용하여 최적의 선택을 할 확률은 약 *36.8%* 가 됩니다.

## 강화학습을 적용하여 최적해 확인하기
제가 제시한 정책은 $k$명은 무조건 고용을 거절하는 정책입니다.  
여기서 $k$값의 최적해를 찾기 위해, $k$값을 1 ~ $n$ 으로 하는 $n$개의 슬롯머신 중  
어떤 슬롯머신을 선택하는게 가장 현명한지 학습하는 Multi-Armed-Bandit 문제로 모델링 할 수 있습니다.
  
<br>
  
### [환경 마련] 

<div align="center">
  <table>
    <tr>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/c0b121ba-3f7d-4522-b388-8251d1f5f9e3" width="960px" height="540px"/></th>
    </tr>
  </table>
</div>
  
Unity를 통해 MAB를 위한 환경을 제작했습니다.  
$n$값은 100으로 하여 Action이 100가지, State는 1가지인 환경으로 제작했습니다.    
  
<br>

<div align="center">
  <table>
    <tr>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/3c26a997-df2a-4cc2-aa50-094c2fc53f18" width="500px" height="300px"/></th>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/c6206794-65e7-4ea7-909c-515365214026" width="500px" height="300px"/></th>
    </tr>
    <tr>
      <td align="center">▲ 면접관 (Agent) </td>
      <td align="center">▲ 최고의 지원자</td>
    </tr>
  </table>
</div>
  
Agent는 면접관의 역할을 수행합니다.  
Agent는 k값에 따라 다음 Action을 수행합니다. $(1 \leq k \leq 100)$  

- $k$번째 지원자까지 무조건 고용을 거절합니다.
- $k$번째까지의 지원자보다 뛰어난 지원자가 나타나면 고용합니다.  
  
<br>

<div align="center">
  <table>
    <tr>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/22ca4437-eaf8-4fd2-b5a2-4af813fe9ed4" width="500px" height="300px"/></th>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/6c513f8d-1e7c-4916-9698-3e7cad27dd8b" width="500px" height="300px"/></th>
    </tr>
    <tr>
      <td align="center">▲ 성공</td>
      <td align="center">▲ 실패</td>
    </tr>
  </table>
</div>
  
최고의 지원자 고용 성공 여부에 따라 Material을 교체하여 결과를 확인할 수 있도록 했습니다.  

### [Epsilon-Greedy 방식 적용]
Explore와 Exploit 사이의 Trade-Off로 인하여 항상 Exploit하지 않고,  
일정 확률로 Explore 하여 0.5의 확률로 선택 가능한 모든 Action중 무작위로 하나를 선택하는 방식을 적용하였습니다.
  
### [Softmax 방식 적용]
Softmax 함수를 통해 Action을 선택하여 가치가 높은 행동들에 대해 더 자주 탐색하도록 하였습니다.  

### [Upper Confidence Bound 방식 적용]
불확실성을 고려하여 신뢰구간에 따라 Action을 선택하도록 했습니다.  
UCB를 적용한 방식은 Action의 가치를 잘 모르는 초기 학습 단계에서 특히 유용 했습니다.

## 강화학습을 적용하여 최적 정책 확인하기
이번에는 이미 수학적으로 증명한 최적 정책을 적용하지 않고,  
State만 주어지는 상황에서 강화학습을 적용하고 결과를 확인해보겠습니다.

### [환경 마련]
우리는 다음과 같이 State를 정의하여 한 명식 순서대로 면접을 진행하는 환경을 마련할 수 있습니다.

1. 지원자 총원 $n$
2. 현재 면접을 보는 지원자를 포함하여 지금까지 면접을 진행한 인원 $[1,n]$
3. 지금까지의 면접을 반영한 현재 지원자의 순위 [1,n]

이때 지원자 총원은 모든 Episode에서 하나의 값으로 고정되기 때문에  
실질적인 state는 $n * n = n^2$ 가지 입니다.

면접관이 한 명의 지원자와 인터뷰를 진행하는 것을 1번의 step이라고 하면,  
1번의 step에서 선택할 수 있는 Action은 다음 2가지로 정의할 수 있습니다.

1. Select : 해당 면접자를 고용하기로 결정합니다.  
2. Pass : 해당 면접자를 고용하지 않기로 결정합니다.

Select를 선택하는 경우 이후의 면접은 진행하지 않고 terminate state로 상태가 전이됩니다.  
지원자 중 가장 뛰어난 비서를 고용하게 된 경우 Reward +1, 아닌 경우 Reward +0 이 부여됩니다.

Pass를 선택하는 경우 다음 면접자의 인터뷰가 진행합니다.  
만약 마지막 지원자까지 Pass 하는 경우 Reward +0이 부여됩니다.

### [Markov Decision Process]
State와 Action에 대한 정의가 이뤄졌습니다. 그리고 해당 문제는 보상함수와 전이확률을 정의할 수 있습니다.  
따라서 해당 문제는 MDP를 정의할 수 있습니다.  

하지만 State만 주어지는 상황에서 직관적인 결과 확인을 위해 MDP를 모른다고 가정하고 프로젝트를 진행했습니다.


### [Monte Carlo Prediction]
Prediction은 정책이 주어졌을 때 State의 Value를 평가하는 문제입니다.  
하지만 현재 지원자의 순위가 1이고, 면접을 진행한 인원이 많을 수록 상태의 가치가 높다는 것은 직관적으로 알 수 있습니다.

따라서 Prediction은 적용하지 않았습니다.


### [Monte Carlo Control]
최적의 정책을 확인하기 위해 Control로 문제를 해결하였습니다.

Monte Carlo Control을 적용하기 위해 $Q$테이블을 운용했습니다.
한 Episode의 경험을 쌓고, 경험한 데이터로 $Q(s,a)$ 테이블의 값을 업데이트 합니다.

epsilon greedy를 통해 확률적으로 Explore하도록 하고,
Exploit하는 경우 $Q(s,a)$ 테이블을 통해 Action을 선택합니다.  

해당 과정의 결과로 얻은 정책의 결과물은 다음과 같습니다.

<div align="center">
  <table>
    <tr>
      <th><img src="https://github.com/oeccsy/SecretaryProblem/assets/77562357/e2dd11bd-4ab2-4058-9fb1-9d7d2deb1457" width="500px" height="300px"/></th>
    </tr>
    <tr>
      <td align="center">▲ 정책 ( 0 : Select, 1 : Pass )</td>
    </tr>
  </table>
</div>

해당 결과로부터, 강화학습을 통해 확인된 정책이 수학적으로 증명된 최적 정책과 거의 일치함을 알 수 있었습니다.  

- 1열의 값에 0(Select)이 분포
    - 지금까지의 인터뷰 중 지원자의 순위가 1인 경우에만 Select 해야 함이 반영된 것으로 보입니다.

- 1열 중 앞의 3행은 1(Pass)이 분포
    - 10명중 3명은 무조건 Pass 해야 한다는 증명된 최적 정책과 일치합니다.

- 마지막 10행에 모두 0이 분포
    - 10번째 지원자는 더이상 Pass 할 수 없다는 것이 반영된 것으로 보입니다.

- 상부 삼각 행렬에 0이 분포
    - n번째 지원자가 n번째 보다 뒷순위인 경우가 발생할 수 없기 때문에 초기화 값이 그대로 반영되어 있는 것으로 보입니다.

- [9,8]에 0값이 하나 존재
    - 인터뷰 순서가 뒤일 수록 인터뷰의 기회가 줄어들고, Reward가 발생하기 까다롭기 때문에 나타난 현상으로 보입니다.  
    - 충분한 episode의 수로 학습을 진행한다면 우리가 원하는 최적 정책을 따라 1로 덮이는 상태가 될 것으로 예상합니다.  
