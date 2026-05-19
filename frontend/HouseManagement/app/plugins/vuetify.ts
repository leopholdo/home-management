import '@mdi/font/css/materialdesignicons.css'
import '@/styles/vuetify/_index.scss'
import 'vuetify/styles'

import { createVuetify } from 'vuetify'
import * as directives from 'vuetify/directives'
import { createRulesPlugin } from 'vuetify/labs/rules'
import { VIconBtn } from 'vuetify/labs/VIconBtn'
import { pt, en } from 'vuetify/locale'

const darkTheme = {
  name: 'darkTheme',
  dark: true,
  colors: {
    // Backgrounds
    background: '#121212',
    surface: '#242424',
    'surface-bright': '#2B2B2B',
    'surface-light': '#303030',

    // Text
    'on-background': '#FFFFFF',
    'on-surface': '#FFFFFF',
    'on-primary': '#0F1115',

    // UI colors
    primary: '#158aca',
    secondary: '#7BE36B',
    success: '#6FD35D',
    error: '#FF6B6B',
    warning: '#FFB74D',
    info: '#64B5F6',
    danger: '#FF7A6B',

    // Borders / dividers
    outline: '#3A3A3A',

    // Custom
    card: '#343434',
    navbar: '#1D1D1D',
    divider: '#454545',
    progressTrack: '#353535',
  },

  variables: {
    'border-color': '#3A3A3A',
    'border-opacity': 0.12,

    'high-emphasis-opacity': 1,
    'medium-emphasis-opacity': 0.72,
    'disabled-opacity': 0.38,

    'theme-background': '#121212',
    'theme-surface': '#242424',

    'shadow-key-umbra-opacity': '0.18',
    'shadow-key-penumbra-opacity': '0.14',
    'shadow-key-ambient-opacity': '0.12',
  },
}

const defaults = {
  VCard: {
    variant: 'tonal',
  },
  VTextField: {
    density: 'comfortable',
    hideDetails: 'auto',
    rounded: 'lg',
  },
  VSelect: {
    density: 'comfortable',
    hideDetails: 'auto',
    rounded: 'lg',
    variant: 'solo-filled',
  },
  IconBtn: {
    icon: true,
    size: 'small',
    color: 'default',
    density: 'comfortable',
    variant: 'text',
  },
  VBtn: {
    class: 'rounded-lg',
  },
}

export default defineNuxtPlugin((app) => {
  const vuetify = createVuetify({
    directives,
    defaults,
    components: {
      VIconBtn,
    },
    locale: {
      locale: 'pt',
      fallback: 'en',
      messages: {
        pt,
        en,
      },
    },
    theme: {
      defaultTheme: 'darkTheme',
      themes: {
        light: {
          dark: false,
          colors: {
            primary: '#729aa2',
          },
        },
        darkTheme,
      },
    },
  })
  app.vueApp.use(vuetify)
  app.vueApp.use(createRulesPlugin({}, vuetify.locale))
})
