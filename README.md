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
